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
        var response = result.Message.Value;
        Logger.LogDebug($"OrderTrades received: {response}");

        var portfolioId = await OrderEventRepository.GetPortfolioIdByOrderId(response.OrderId);

        if (portfolioId == null)
        {
            // Если не нашли portfolioId, значит ордер был выставлен в обход этого API
            Logger.LogWarning($"Cannot get portfolio identity for OrderId={response.OrderId}. Using OrderTrades.AccountId");

            if (string.IsNullOrEmpty(response.AccountId))
            {
                Logger.LogWarning($"Cannot get AccountId for OrderId={response.OrderId}. OrderTrades.AccountId is empty. Skipping message.");
                return;
            }

            portfolioId = new Contracts.PortfolioIdentity(response.AccountId);
        }

        var orderEvent = response.CreateEvent(portfolioId);
        var saved = await OrderEventRepository.Save(orderEvent);

        if (!saved)
        {
            Logger.LogWarning($"Cannot save OrderTrades event for OrderId={orderEvent.OrderId}");
        }

        var operations = response.CreateOperations(portfolioId);

        Logger.LogDebug($"Publish OrderTrades operations for OrderId={response.OrderId}");
        await OperationsPublisher.Publish(operations);
    }
}
