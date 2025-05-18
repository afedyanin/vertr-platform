using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Vertr.Infrastructure.Kafka;
using Vertr.Infrastructure.Kafka.Abstractions;
using Vertr.OrderExecution.Contracts;
using Vertr.PortfolioManager.Application.Abstractions;
using Vertr.PortfolioManager.Converters;

namespace Vertr.PortfolioManager.BackgroundServices;

public class OrderOperationConsumerService : BackgroundService
{
    private readonly PortfolioManagerSettings _settings;
    private readonly KafkaSettings _kafkaSettings;
    private readonly ILogger<OrderOperationConsumerService> _logger;
    private readonly IConsumerWrapper<string, OrderOperation> _consumer;
    private readonly IOperationEventRepository _operationEventRepository;
    private readonly string? _topicName;


    public OrderOperationConsumerService(
        IOptions<PortfolioManagerSettings> settings,
        IOptions<KafkaSettings> kafkaSettings,
        IConsumerWrapper<string, OrderOperation> consumer,
        IOperationEventRepository operationEventRepository,
        ILogger<OrderOperationConsumerService> logger)
    {
        _settings = settings.Value;
        _kafkaSettings = kafkaSettings.Value;
        _consumer = consumer;
        _logger = logger;
        _topicName = _kafkaSettings.GetTopicByKey(_settings.OperationsTopicKey);
        _operationEventRepository = operationEventRepository;
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

    private async Task HandleOrderOperationMessage(ConsumeResult<string, OrderOperation> result, CancellationToken token)
    {
        var response = result.Message.Value;

        _logger.LogInformation($"Order operation received: {response}");

        var operationEvent = response.Convert();

        if (operationEvent == null)
        {
            _logger.LogWarning($"Cannot convert operation event: {response}");
            return;
        }

        var saved = await _operationEventRepository.Save(operationEvent);

        if (!saved)
        {
            _logger.LogWarning($"Cannot save operation event: {response}");
        }

        // TODO: Implement operation event handling
    }
}
