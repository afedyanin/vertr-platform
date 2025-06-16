using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.TinvestGateway.Application.Converters;
using Vertr.TinvestGateway.Application.Settings;
using Vertr.TinvestGateway.Contracts.Enums;
using Vertr.TinvestGateway.Contracts.Requests;

namespace Vertr.TinvestGateway.Application.BackgroundServices;

public class MarketDataStreamService : StreamServiceBase
{
    protected override bool IsEnabled => TinvestSettings.MarketDataStreamEnabled;

    public MarketDataStreamService(
        IOptions<TinvestSettings> tinvestOptions,
        InvestApiClient investApiClient,
        IMediator mediator,
        ILogger<MarketDataStreamService> logger) :
            base(tinvestOptions, investApiClient, mediator, logger)
    {
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

        foreach (var kvp in TinvestSettings.Candles)
        {
            candleRequest.Instruments.Add(new Tinkoff.InvestApi.V1.CandleInstrument()
            {
                InstrumentId = TinvestSettings.GetSymbolId(kvp.Key),
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
                var instrumentId = response.Candle.InstrumentUid;

                var symbol = TinvestSettings.GetSymbolById(instrumentId);

                if (string.IsNullOrEmpty(symbol))
                {
                    logger.LogWarning($"Unknown candle received: InstrumentUid={instrumentId}");
                    continue;
                }

                var interval = TinvestSettings.GetInterval(symbol);

                if (interval == CandleInterval.Unspecified)
                {
                    logger.LogWarning($"Cannot define candle interval for Symbol={symbol} InstrumentUid={instrumentId}");
                    continue;
                }

                var marketDataCandleRequest = new HandleMarketDataCandleRequest
                {
                    Candle = response.Candle.Convert(),
                    CandleInterval = interval,
                };

                await Mediator.Send(marketDataCandleRequest, stoppingToken);
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
