
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Vertr.Infrastructure.Kafka;
using Vertr.Infrastructure.Kafka.Abstractions;
using Vertr.PortfolioManager.Application.Abstractions;
using Vertr.PortfolioManager.Application.Converters;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.BackgroundServices;

public class PortfolioConsumerService : BackgroundService
{
    private readonly PortfolioManagerSettings _settings;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IPortfolioSnapshotRepository _snapshotRepository;
    private readonly ILogger<PortfolioConsumerService> _logger;
    private readonly IConsumerWrapper<string, PortfolioResponse> _consumer;
    private readonly string? _topicName;

    public PortfolioConsumerService(
        IOptions<PortfolioManagerSettings> settings,
        IOptions<KafkaSettings> kafkaSettings,
        IPortfolioSnapshotRepository snapshotRepository,
        IConsumerWrapper<string, PortfolioResponse> consumer,
        ILogger<PortfolioConsumerService> logger)
    {
        _settings = settings.Value;
        _kafkaSettings = kafkaSettings.Value;
        _snapshotRepository = snapshotRepository;
        _consumer = consumer;
        _logger = logger;
        _topicName = _kafkaSettings.GetTopicByKey(_settings.PortfoliosTopicKey);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_settings.IsPortfolioConsumerEnabled || string.IsNullOrEmpty(_topicName))
        {
            _logger.LogWarning($"Skip starting {nameof(PortfolioConsumerService)}");
            return Task.CompletedTask;
        }

        return _consumer.Consume([_topicName], HandlePortfolioMessage, readFromBegining: false, stoppingToken);
    }

    private async Task HandlePortfolioMessage(ConsumeResult<string, PortfolioResponse> result, CancellationToken token)
    {
        var response = result.Message.Value;

        _logger.LogDebug($"Portfolio snapshot received: {response}");

        var snapshot = response.Convert(bookId: null);

        if (snapshot == null)
        {
            _logger.LogWarning($"Cannot convert portfolio snapshot: {response}");
            return;
        }

        var saved = await _snapshotRepository.Save(snapshot);

        if (!saved)
        {
            _logger.LogWarning($"Cannot save portfolio snapshot: {response}");
        }
    }
}
