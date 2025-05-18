
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Options;
using Vertr.Infrastructure.Kafka;
using Vertr.Infrastructure.Kafka.Abstractions;
using Vertr.MarketData.Application.Commands;
using Vertr.MarketData.Converters;
using Vertr.TinvestGateway.Contracts;

namespace Vertr.MarketData.BackgroundServices;

public class TinvestMarketDataStreamConsumer : BackgroundService
{
    private readonly MarketDataSettings _settings;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IMediator _mediator;
    private readonly ILogger<TinvestMarketDataStreamConsumer> _logger;
    private readonly IConsumerWrapper<string, Candle> _consumer;
    private readonly string? _topicName;

    public TinvestMarketDataStreamConsumer(
        IOptions<MarketDataSettings> settings,
        IOptions<KafkaSettings> kafkaSettings,
        IMediator mediator,
        IConsumerWrapper<string, Candle> consumer,
        ILogger<TinvestMarketDataStreamConsumer> logger)
    {
        _settings = settings.Value;
        _kafkaSettings = kafkaSettings.Value;
        _logger = logger;
        _consumer = consumer;
        _mediator = mediator;
        _topicName = _kafkaSettings.GetTopicByKey(_settings.TinvestMarketDataTopicKey);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_settings.IsTinvestMarketDataConsumerEnabled || string.IsNullOrEmpty(_topicName))
        {
            _logger.LogWarning($"Skip starting {nameof(TinvestMarketDataStreamConsumer)}");
            return Task.CompletedTask;
        }

        return _consumer.Consume([_topicName], HandleMarketDataMessage, readFromBegining: false, stoppingToken);
    }

    private async Task HandleMarketDataMessage(ConsumeResult<string, Candle> result, CancellationToken token)
    {
        var response = result.Message.Value;
        _logger.LogDebug($"Tinvest Market data received: {response}");

        var candle = response.Convert();

        if (candle == null)
        {
            _logger.LogError($"Cannot convert candle: {response}");
            return;
        }

        var requet = new NewCandleRequest { Candle = candle };
        await _mediator.Send(requet, token);
    }
}
