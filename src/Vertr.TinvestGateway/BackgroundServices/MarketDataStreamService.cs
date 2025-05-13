using Grpc.Core;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;

namespace Vertr.TinvestGateway.BackgroundServices;

public class MarketDataStreamService : StreamServiceBase
{
    public MarketDataStreamService(
        IOptions<TinvestSettings> options,
        InvestApiClient investApiClient,
        ILogger<MarketDataStreamService> logger) : base(options, investApiClient, logger)
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
            WaitingClose = true,
        };

        candleRequest.Instruments.Add(new Tinkoff.InvestApi.V1.CandleInstrument()
        {
            InstrumentId = Settings.GetSymbolId("SBER"),
            Interval = Tinkoff.InvestApi.V1.SubscriptionInterval.OneMinute
        });

        var request = new Tinkoff.InvestApi.V1.MarketDataServerSideStreamRequest()
        {
            SubscribeCandlesRequest = candleRequest,
        };

        using var stream = InvestApiClient.MarketDataStream.MarketDataServerSideStream(request, headers: null, deadline, stoppingToken);

        await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
        {
            if (response.PayloadCase == Tinkoff.InvestApi.V1.MarketDataResponse.PayloadOneofCase.Candle)
            {
                var candle = response.Candle;
                logger.LogInformation($"Candle received: Time={candle.Time:O} Open={candle.Open} Close={candle.Close} Vol={candle.Volume} LastTradeTime={candle.LastTradeTs:O} Interval={candle.Interval} Id={candle.InstrumentUid}");
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
