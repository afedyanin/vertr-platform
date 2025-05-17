using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Vertr.Infrastructure.Kafka;
using Vertr.Infrastructure.Kafka.Abstractions;
using Vertr.OrderExecution.Contracts;

namespace Vertr.PortfolioManager.BackgroundServices;

public class OrderOperationConsumerService : BackgroundService
{
    private readonly PortfolioManagerSettings _settings;
    private readonly KafkaSettings _kafkaSettings;
    private readonly ILogger<OrderOperationConsumerService> _logger;
    private readonly IConsumerWrapper<string, OrderOperation> _consumer;
    private readonly string? _topicName;


    public OrderOperationConsumerService(
        IOptions<PortfolioManagerSettings> settings,
        IOptions<KafkaSettings> kafkaSettings,
        IConsumerWrapper<string, OrderOperation> consumer,
        ILogger<OrderOperationConsumerService> logger)
    {
        _settings = settings.Value;
        _kafkaSettings = kafkaSettings.Value;
        _consumer = consumer;
        _logger = logger;
        _topicName = _kafkaSettings.GetTopicByKey(_settings.OperationsTopicKey);
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_settings.IsOrderOperationConsumerEnabled || string.IsNullOrEmpty(_topicName))
        {
            _logger.LogWarning($"Skip starting {nameof(OrderOperationConsumerService)}");
            return Task.CompletedTask;
        }

        return _consumer.Consume([_topicName], HandleOrderOperationMessage, readFromBegining: false, stoppingToken);
    }

    private Task HandleOrderOperationMessage(ConsumeResult<string, OrderOperation> result, CancellationToken token)
    {
        var response = result.Message.Value;

        _logger.LogInformation($"Order operation received: {response}");

        // TODO: Implement this
        return Task.CompletedTask;
    }
}
