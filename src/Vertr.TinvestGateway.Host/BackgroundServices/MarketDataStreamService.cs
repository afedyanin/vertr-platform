using Grpc.Core;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.Common.DataAccess.Repositories;
using Vertr.TinvestGateway.Abstractions;
using Vertr.TinvestGateway.Converters;

namespace Vertr.TinvestGateway.Host.BackgroundServices;

public class MarketDataStreamService : StreamServiceBase
{
    private const int CandlesHistoryLoadMaxDays = 5;

    protected override bool IsEnabled => TinvestSettings.MarketDataStreamEnabled;

    private readonly Dictionary<Guid, int> _candleLimits = [];

    public MarketDataStreamService(
        IServiceProvider serviceProvider,
        IOptions<TinvestSettings> tinvestOptions,
        ILogger<MarketDataStreamService> logger) : base(serviceProvider, tinvestOptions, logger)
    {
    }

    protected override async Task OnBeforeStart(CancellationToken stoppingToken)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var marketDataGateway = scope.ServiceProvider.GetRequiredService<IMarketDataGateway>();
        var instrumentRepository = scope.ServiceProvider.GetRequiredService<IInstrumentRepository>();

        foreach (var sub in TinvestSettings.Subscriptions)
        {
            var instrument = await marketDataGateway.GetInstrumentById(sub.InstrumentId);

            if (instrument == null)
            {
                continue;
            }

            _candleLimits[sub.InstrumentId] = sub.MaxCount;
            await instrumentRepository.Save(instrument);
        }

        foreach (var kvp in TinvestSettings.Currencies)
        {
            var currency = await marketDataGateway.GetInstrumentById(kvp.Value);

            if (currency == null)
            {
                continue;
            }

            currency.Ticker = kvp.Key;

            await instrumentRepository.Save(currency);
        }

        await ReloadMarketData(scope);
    }

    private async Task ReloadMarketData(AsyncServiceScope scope)
    {
        var marketDataGateway = scope.ServiceProvider.GetRequiredService<IMarketDataGateway>();
        var candlestickRepository = scope.ServiceProvider.GetRequiredService<ICandlestickRepository>();

        // Reinit market data
        foreach (var sub in TinvestSettings.Subscriptions)
        {
            // cleanup market data
            await candlestickRepository.Clear(sub.InstrumentId);

            // load market data
            var totalSavedCandles = 0L;
            for (var i = 0; i < CandlesHistoryLoadMaxDays; i++)
            {
                var day = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-i));
                var candlesByDay = await marketDataGateway.GetCandles(sub.InstrumentId, day, sub.Interval);
                var savedCountByDay = await candlestickRepository.Save(sub.InstrumentId, candlesByDay, sub.MaxCount, publish: false);
                totalSavedCandles += savedCountByDay;

                Logger.LogInformation("Historic candles loaded for InstrumentId={InstrumentId} Total={Total}", sub.InstrumentId, totalSavedCandles);

                if (sub.MaxCount <= 0 || totalSavedCandles >= sub.MaxCount)
                {
                    break;
                }
            }
        }
    }

    protected override async Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var investApiClient = scope.ServiceProvider.GetRequiredService<InvestApiClient>();
        var candlestickRepository = scope.ServiceProvider.GetRequiredService<ICandlestickRepository>();
        var orderBookRepository = scope.ServiceProvider.GetRequiredService<IOrderBookRepository>();

        var candleRequest = new Tinkoff.InvestApi.V1.SubscribeCandlesRequest
        {
            SubscriptionAction = Tinkoff.InvestApi.V1.SubscriptionAction.Subscribe,
            WaitingClose = TinvestSettings.WaitCandleClose,
        };

        var orderBookRequest = new Tinkoff.InvestApi.V1.SubscribeOrderBookRequest
        {
            SubscriptionAction = Tinkoff.InvestApi.V1.SubscriptionAction.Subscribe,
        };

        foreach (var sub in TinvestSettings.Subscriptions)
        {
            if (sub.Disabled)
            {
                logger.LogInformation($"Skipping candle subscription: InstrumentId={sub.InstrumentId} Interval={sub.Interval} Disabled={sub.Disabled}");
                continue;
            }

            logger.LogInformation("Adding candle subscription: InstrumentId={InstrumentId} Interval={Interval}", sub.InstrumentId, sub.Interval);

            candleRequest.Instruments.Add(new Tinkoff.InvestApi.V1.CandleInstrument()
            {
                InstrumentId = sub.InstrumentId.ToString(),
                Interval = sub.Interval.ConvertToSubscriptionInterval()
            });

            orderBookRequest.Instruments.Add(new Tinkoff.InvestApi.V1.OrderBookInstrument()
            {
                InstrumentId = sub.InstrumentId.ToString(),
                Depth = sub.OrderBookDepth,
                OrderBookType = Tinkoff.InvestApi.V1.OrderBookType.All,
            });
        }

        var request = new Tinkoff.InvestApi.V1.MarketDataServerSideStreamRequest()
        {
            SubscribeCandlesRequest = candleRequest,
            SubscribeOrderBookRequest = orderBookRequest,
        };

        using var stream = investApiClient.MarketDataStream.MarketDataServerSideStream(request, headers: null, deadline, stoppingToken);

        await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
        {
            if (response.PayloadCase == Tinkoff.InvestApi.V1.MarketDataResponse.PayloadOneofCase.Candle)
            {
                var instrumentId = new Guid(response.Candle.InstrumentUid);
                _candleLimits.TryGetValue(instrumentId, out var maxCount);
                var candle = response.Candle.ToCandlestick();
                await candlestickRepository.Save(instrumentId, [candle], maxCount);
                logger.LogInformation("Candle subscriptions received: candle={Candle}", candle);
            }
            else if (response.PayloadCase == Tinkoff.InvestApi.V1.MarketDataResponse.PayloadOneofCase.SubscribeCandlesResponse)
            {
                var subs = response.SubscribeCandlesResponse;
                var all = subs.CandlesSubscriptions.ToArray();

                logger.LogInformation($"Candle subscriptions received: TrackingId={subs.TrackingId} Details={string.Join(',',
                    [.. all.Select(s => $"Id={s.SubscriptionId} Status={s.SubscriptionStatus} Instrument={s.InstrumentUid} Inverval={s.Interval}")])}");
            }
            else if (response.PayloadCase == Tinkoff.InvestApi.V1.MarketDataResponse.PayloadOneofCase.Ping)
            {
                logger.LogDebug("Candle ping received: {Ping}", response.Ping);
            }
            else if (response.PayloadCase == Tinkoff.InvestApi.V1.MarketDataResponse.PayloadOneofCase.Orderbook)
            {
                var ob = response.Orderbook.Convert();
                await orderBookRepository.Save(ob);
                logger.LogDebug("Order book received: {OrderBook}", ob);
            }
            else if (response.PayloadCase == Tinkoff.InvestApi.V1.MarketDataResponse.PayloadOneofCase.SubscribeOrderBookResponse)
            {
                var subs = response.SubscribeOrderBookResponse;
                var all = subs.OrderBookSubscriptions.ToArray();

                logger.LogInformation($"Order book subscriptions received: TrackingId={subs.TrackingId} Details={string.Join(',',
                    [.. all.Select(s => $"Id={s.SubscriptionId} Status={s.SubscriptionStatus} Instrument={s.InstrumentUid} Depth={s.Depth}")])}");
            }
        }
    }
}