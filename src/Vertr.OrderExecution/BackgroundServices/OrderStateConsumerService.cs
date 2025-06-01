using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Vertr.Infrastructure.Kafka;
using Vertr.Infrastructure.Kafka.Abstractions;
using Vertr.OrderExecution.Application.Abstractions;
using Vertr.OrderExecution.Application.Factories;
using Vertr.TinvestGateway.Contracts;

namespace Vertr.OrderExecution.BackgroundServices;

public class OrderStateConsumerService : ConsumerServiceBase
{
    private readonly string? _topicName;

    private readonly IConsumerWrapper<string, OrderState> _consumer;

    protected override bool IsEnabled => OrderExecutionSettings.IsOrderStateConsumerEnabled;

    public OrderStateConsumerService(
        IOptions<KafkaSettings> kafkaSettings,
        IOptions<OrderExecutionSettings> orderExecutionSettings,
        IOrderEventRepository orderEventRepository,
        IOperationsPublisher operationsPublisher,
        IConsumerWrapper<string, OrderState> consumer,
        ILogger logger)
        : base(kafkaSettings, orderExecutionSettings, orderEventRepository, operationsPublisher, logger)
    {
        _consumer = consumer;
        _topicName = KafkaSettings.GetTopicByKey(OrderExecutionSettings.OrderStateTopicKey);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!IsEnabled || string.IsNullOrEmpty(_topicName))
        {
            Logger.LogWarning($"Skip starting {nameof(OrderStateConsumerService)}");
            return Task.CompletedTask;
        }

        return _consumer.Consume([_topicName], HandleOrderStateMessage, readFromBegining: false, stoppingToken);
    }

    private async Task HandleOrderStateMessage(ConsumeResult<string, OrderState> result, CancellationToken token)
    {
        var response = result.Message.Value;

        Logger.LogDebug($"OrderState received: {response}");

        var portfolioId = await OrderEventRepository.GetPortfolioIdByOrderId(response.OrderId);

        if (portfolioId == null)
        {
            // Если не нашли portfolioId, значит ордер был выставлен в обход этого API
            // TODO: Использовать OrderState.AccountId, когда он появится в SDK. См. обработку OrderTrades
            Logger.LogWarning($"Cannot get portfolio identity for OrderId={response.OrderId}. Skipping message.");
            return;
        }

        var orderEvent = response.CreateEvent(portfolioId);
        var saved = await OrderEventRepository.Save(orderEvent);

        if (!saved)
        {
            Logger.LogWarning($"Cannot save OrderState event for OrderId={orderEvent.OrderId}");
        }

        var operations = response.CreateOperations(portfolioId);

        Logger.LogDebug($"Publish OrderState operations for OrderId={response.OrderId}");
        await OperationsPublisher.Publish(operations);
    }
}
