using Microsoft.Extensions.Options;
using Vertr.Infrastructure.Kafka;
using Vertr.Infrastructure.Kafka.Abstractions;
using Vertr.OrderExecution.Application.Abstractions;
using Vertr.OrderExecution.Contracts;

namespace Vertr.OrderExecution.BackgroundServices;

public class KafkaOperationsPublisher : IOperationsPublisher
{
    private readonly IProducerWrapper<string, OrderOperation> _producerWrapper;
    private readonly KafkaSettings _kafkaSettings;
    private readonly OrderExecutionSettings _orderExecutionSettings;
    private readonly ILogger<KafkaOperationsPublisher> _logger;

    private readonly string? _topicName;

    public KafkaOperationsPublisher(
        IProducerWrapper<string, OrderOperation> producerWrapper,
        IOptions<KafkaSettings> kafkaSettings,
        IOptions<OrderExecutionSettings> orderExecutionSettings,
        ILogger<KafkaOperationsPublisher> logger)
    {
        _producerWrapper = producerWrapper;
        _kafkaSettings = kafkaSettings.Value;
        _orderExecutionSettings = orderExecutionSettings.Value;
        _topicName = _kafkaSettings.GetTopicByKey(_orderExecutionSettings.OperationsTopicKey);
        _logger = logger;
    }

    public async Task Publish(
        OrderOperation[] operations,
        CancellationToken cancellationToken = default)
    {
        if (!_orderExecutionSettings.IsOperationsProducerEnabled)
        {
            _logger.LogWarning($"Operations publishing is disabled.");
            return;
        }

        if (string.IsNullOrEmpty(_topicName))
        {
            _logger.LogWarning($"Skipping producing to Kafka. Unknown topic name.");
            return;
        }

        foreach (var operation in operations)
        {
            _logger.LogDebug($"Publishing operation: {operation}");

            _ = await _producerWrapper.Produce(
                _topicName,
                operation.OperationType.ToString(),
                operation,
                DateTime.UtcNow,
                cancellationToken);
        }
    }
}
