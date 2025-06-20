using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Vertr.Infrastructure.Kafka;
using Vertr.Infrastructure.Kafka.Abstractions;
using Vertr.OrderExecution.Application.Abstractions;
using Vertr.OrderExecution.Application.Factories;
using Vertr.TinvestGateway.Contracts;

namespace Vertr.OrderExecution.BackgroundServices;

public class OrderTradesConsumerService : ConsumerServiceBase
{
    private readonly string? _topicName;

    private readonly IConsumerWrapper<string, OrderTrades> _consumer;

    protected override bool IsEnabled => OrderExecutionSettings.IsOrderTradesConsumerEnabled;

    public OrderTradesConsumerService(
        IOptions<KafkaSettings> kafkaSettings,
        IOptions<OrderExecutionSettings> orderExecutionSettings,
        IOrderEventRepository orderEventRepository,
        IOperationsPublisher operationsPublisher,
        IConsumerWrapper<string, OrderTrades> consumer,
        ILogger logger)
        : base(kafkaSettings, orderExecutionSettings, orderEventRepository, operationsPublisher, logger)
    {
        _consumer = consumer;
        _topicName = KafkaSettings.GetTopicByKey(OrderExecutionSettings.OrderTradesTopicKey);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!IsEnabled || string.IsNullOrEmpty(_topicName))
        {
            Logger.LogWarning($"Skip starting {nameof(OrderTradesConsumerService)}");
            return Task.CompletedTask;
        }

        return _consumer.Consume([_topicName], HandleOrderTradesMessage, readFromBegining: false, stoppingToken);
    }
    private async Task HandleOrderTradesMessage(ConsumeResult<string, OrderTrades> result, CancellationToken token)
    {
    }
}
