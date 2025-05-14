using Grpc.Core;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.Infrastructure.Kafka;
using Vertr.Infrastructure.Kafka.Abstractions;
using Vertr.TinvestGateway.Contracts;
using Vertr.TinvestGateway.Converters;
using Vertr.TinvestGateway.Settings;

namespace Vertr.TinvestGateway.BackgroundServices;

public class MarketDataStreamService : StreamServiceBase
{
    private readonly MarketDataStreamSettings _streamSettings;
    private readonly IProducerWrapper<string, Candle> _producerWrapper;
    private readonly string? _topicName;

    protected override bool IsEnabled => _streamSettings.IsEnabled;

    public MarketDataStreamService(
        IOptions<MarketDataStreamSettings> streamSettings,
        IProducerWrapper<string, Candle> producerWrapper,
        IOptions<KafkaSettings> kafkaSettings,
        IOptions<TinvestSettings> tinvestOptions,
        InvestApiClient investApiClient,
        ILogger<MarketDataStreamService> logger) :
            base(kafkaSettings, tinvestOptions, investApiClient, logger)
    {
        _streamSettings = streamSettings.Value;
        _producerWrapper = producerWrapper;
        _topicName = KafkaSettings.GetTopicByKey(_streamSettings.TopicKey);
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

                var interval = _streamSettings.GetInterval(symbol);

                if (interval == CandleInterval.Unspecified)
                {
                    logger.LogWarning($"Cannot define candle interval for Symbol={symbol} InstrumentUid={instrumentId}");
                    continue;
                }

                var candle = response.Candle.Convert(symbol, interval, _streamSettings.WaitCandleClose ? true : null);

                logger.LogDebug($"Candle received {candle.Symbol}: Time={candle.TimeUtc:O} Open={candle.Open} Close={candle.Close} Vol={candle.Volume} Interval={candle.Interval} Completed={candle.IsCompleted}");

                if (string.IsNullOrEmpty(_topicName))
                {
                    logger.LogWarning($"Skipping producing to Kafka. Unknown topic name.");
                    continue;
                }

                await _producerWrapper.Produce(_topicName, symbol, candle, DateTime.UtcNow, stoppingToken);
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
