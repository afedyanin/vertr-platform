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

namespace Vertr.TinvestGateway.Application.BackgroundServices;

public class MarketDataStreamService : StreamServiceBase
{
    private readonly IMarketDataService _marketDataService;

    private readonly Dictionary<string, Instrument> _instruments = [];
    private readonly Dictionary<string, CandleInterval> _intervals = [];

    protected override bool IsEnabled => TinvestSettings.MarketDataStreamEnabled;

    public MarketDataStreamService(
        IMarketDataService marketDataService,
        IOptions<TinvestSettings> tinvestOptions,
        InvestApiClient investApiClient,
        IMediator mediator,
        ILogger<MarketDataStreamService> logger) :
            base(tinvestOptions, investApiClient, mediator, logger)
    {
        _marketDataService = marketDataService;
    }

    protected override async Task OnBeforeStart(CancellationToken stoppingToken)
    {
        _instruments.Clear();
        _intervals.Clear();

        var subscriptions = await _marketDataService.GetSubscriptions();

        foreach (var subscription in subscriptions)
        {
            var instrument = await _marketDataService.GetInstrument(subscription.InstrumentIdentity);

            if (instrument != null && instrument.InstrumentIdentity.Id.HasValue)
            {
                var id = instrument.InstrumentIdentity.Id.Value.ToString();
                _instruments[id] = instrument;
                _intervals[id] = subscription.Interval;
            }
        }
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
                        InstrumentIdentity = instrument.InstrumentIdentity,
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
