using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.MarketData.Contracts.Requests;
using Vertr.TinvestGateway.Application.Converters;
using Vertr.TinvestGateway.Application.Settings;
using Vertr.TinvestGateway.Contracts.Interfaces;

namespace Vertr.TinvestGateway.Application.BackgroundServices;

public class MarketDataStreamService : StreamServiceBase
{
    private readonly IMarketDataService _marketDataService;
    private readonly ITinvestGatewayMarketData _tinvestGatewayMarketData;

    private readonly Dictionary<string, Instrument> _instruments = new Dictionary<string, Instrument>();
    private readonly Dictionary<string, CandleInterval> _intervals = new Dictionary<string, CandleInterval>();

    protected override bool IsEnabled => TinvestSettings.MarketDataStreamEnabled;

    public MarketDataStreamService(
        ITinvestGatewayMarketData tinvestGatewayMarketData,
        IMarketDataService marketDataService,
        IOptions<TinvestSettings> tinvestOptions,
        InvestApiClient investApiClient,
        IMediator mediator,
        ILogger<MarketDataStreamService> logger) :
            base(tinvestOptions, investApiClient, mediator, logger)
    {
        _tinvestGatewayMarketData = tinvestGatewayMarketData;
        _marketDataService = marketDataService;
    }

    protected override async Task OnBeforeStart(CancellationToken stoppingToken)
    {
        var subscriptions = await _marketDataService.GetSubscriptions();
        _instruments.Clear();

        foreach (var subscription in subscriptions)
        {
            var instrument = await _tinvestGatewayMarketData.GetInstrumentByTicker(subscription.Symbol, subscription.ClassCode);

            if (instrument != null)
            {
                _instruments[instrument.Uid!] = instrument;
                _intervals[instrument.Uid!] = subscription.Interval;
            }
        }

        if (_instruments.Count != 0)
        {
            var request = new InstrumentSnapshotReceived
            {
                Instruments = [.. _instruments.Values]
            };

            await Mediator.Send(request, stoppingToken);
        }

        await base.OnBeforeStart(stoppingToken);
    }

    protected override async Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default)
    {
        var candleRequest = new Tinkoff.InvestApi.V1.SubscribeCandlesRequest
        {
            SubscriptionAction = Tinkoff.InvestApi.V1.SubscriptionAction.Subscribe,
            WaitingClose = TinvestSettings.WaitCandleClose,
        };

        foreach (var kvp in _intervals)
        {
            candleRequest.Instruments.Add(new Tinkoff.InvestApi.V1.CandleInstrument()
            {
                InstrumentId = kvp.Key,
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
                if (_instruments.TryGetValue(response.Candle.InstrumentUid, out var instrument))
                {
                    var marketDataCandleRequest = new NewCandleReceived
                    {
                        Candle = response.Candle.Convert(),
                        Interval = _intervals[response.Candle.InstrumentUid],
                        Ticker = $"{instrument.ClassCode}.{instrument.Ticker}"
                    };

                    await Mediator.Send(marketDataCandleRequest, stoppingToken);
                }
                else
                {
                    logger.LogDebug($"Unknown candle received: {response.Candle}");
                }
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
