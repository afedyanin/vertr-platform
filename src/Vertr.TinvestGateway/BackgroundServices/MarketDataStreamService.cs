using Grpc.Core;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.TinvestGateway.Contracts.Settings;
using Vertr.TinvestGateway.Converters;

namespace Vertr.TinvestGateway.BackgroundServices;

public class MarketDataStreamService : StreamServiceBase
{
    private readonly MarketDataStreamSettings _streamSettings;

    public MarketDataStreamService(
        IOptions<TinvestSettings> tinvestOptions,
        IOptions<MarketDataStreamSettings> marketDataOptions,
        InvestApiClient investApiClient,
        ILogger<MarketDataStreamService> logger) : base(tinvestOptions, investApiClient, logger)
    {
        _streamSettings = marketDataOptions.Value;
    }

    protected override async Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default)
    {
        var candleRequest = new Tinkoff.InvestApi.V1.SubscribeCandlesRequest
        {
            SubscriptionAction = Tinkoff.InvestApi.V1.SubscriptionAction.Subscribe,
            WaitingClose = _streamSettings.WaitCandleClose,
        };

        foreach (var kvp in _streamSettings.Candles)
        {
            candleRequest.Instruments.Add(new Tinkoff.InvestApi.V1.CandleInstrument()
            {
                InstrumentId = Settings.GetSymbolId(kvp.Key),
                Interval = kvp.Value.ConvertToSubscriptionInterval()
            });
        }

        var request = new Tinkoff.InvestApi.V1.MarketDataServerSideStreamRequest()
        {
            SubscribeCandlesRequest = candleRequest,
        };

        using var stream = InvestApiClient.MarketDataStream.MarketDataServerSideStream(request, headers: null, deadline, stoppingToken);

        await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
        {
            if (response.PayloadCase == Tinkoff.InvestApi.V1.MarketDataResponse.PayloadOneofCase.Candle)
            {
                var reponseCandle = response.Candle;

                var symbol = Settings.GetSymbolById(reponseCandle.InstrumentUid);

                if (string.IsNullOrEmpty(symbol))
                {
                    logger.LogWarning($"Unregistered candle received: InstrumentUid={reponseCandle.InstrumentUid}");
                    continue;
                }

                var interval = _streamSettings.GetInterval(symbol);

                if (interval == Contracts.CandleInterval.Unspecified)
                {
                    logger.LogWarning($"Cannot define candle interval for Symbol={symbol} InstrumentUid={reponseCandle.InstrumentUid}");
                    continue;
                }

                var candle = response.Candle.Convert(symbol, interval, _streamSettings.WaitCandleClose ? true : null);

                logger.LogInformation($"Candle received {candle.Symbol}: Time={candle.TimeUtc:O} Open={candle.Open} Close={candle.Close} Vol={candle.Volume} Interval={candle.Interval} Completed={candle.IsCompleted}");
            }
            else if (response.PayloadCase == Tinkoff.InvestApi.V1.MarketDataResponse.PayloadOneofCase.SubscribeCandlesResponse)
            {
                var subs = response.SubscribeCandlesResponse;
                var all = subs.CandlesSubscriptions.ToArray()
                    .Select(s => $"Id={s.SubscriptionId} Status={s.SubscriptionStatus} Instrument={s.InstrumentUid} Inverval={s.Interval}").ToArray();

                logger.LogInformation($"Candle subscriptions received: TrackingId={subs.TrackingId} Statuses={string.Join(',', all)}");
            }
            else if (response.PayloadCase == Tinkoff.InvestApi.V1.MarketDataResponse.PayloadOneofCase.Ping)
            {
                logger.LogDebug($"Candle ping received: {response.Ping}");
            }
        }
    }
}
