using Vertr.Infrastructure.Kafka.Abstractions;
using Vertr.Infrastructure.Kafka;
using Vertr.MarketData.Application.Abstractions;
using Vertr.MarketData.Contracts;
using Microsoft.Extensions.Options;

namespace Vertr.MarketData.BackgroundServices;

public class KafkaMarketDataPublisher : IMarketDataPublisher
{
    private readonly IProducerWrapper<string, Candle> _producerWrapper;
    private readonly KafkaSettings _kafkaSettings;
    private readonly MarketDataSettings _marketDataSettings;
    private readonly ILogger<KafkaMarketDataPublisher> _logger;

    private readonly string? _topicName;

    public KafkaMarketDataPublisher(
        IProducerWrapper<string, Candle> producerWrapper,
        IOptions<KafkaSettings> kafkaSettings,
        IOptions<MarketDataSettings> marketDataSettings,
        ILogger<KafkaMarketDataPublisher> logger)
    {
        _producerWrapper = producerWrapper;
        _kafkaSettings = kafkaSettings.Value;
        _marketDataSettings = marketDataSettings.Value;
        _topicName = _kafkaSettings.GetTopicByKey(_marketDataSettings.CandlesMarketDataTopicKey);
        _logger = logger;
    }

    public async Task Publish(Candle candle, CancellationToken cancellationToken = default)
    {
        if (!_marketDataSettings.IsCandlesPublisherEnabled)
        {
            _logger.LogWarning($"Candles publishing is disabled.");
            return;
        }

        if (string.IsNullOrEmpty(_topicName))
        {
            _logger.LogWarning($"Skipping producing to Kafka. Unknown topic name.");
            return;
        }

        _logger.LogDebug($"Publishing candle: {candle}");

        _ = await _producerWrapper.Produce(
            _topicName,
            candle.Symbol,
            candle,
            DateTime.UtcNow,
            cancellationToken);
    }
}
