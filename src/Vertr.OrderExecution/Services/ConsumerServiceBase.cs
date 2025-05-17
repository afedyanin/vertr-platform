using Vertr.Infrastructure.Kafka;
using Microsoft.Extensions.Options;
using Vertr.OrderExecution.Application.Abstractions;

namespace Vertr.OrderExecution.Services;

public abstract class ConsumerServiceBase : BackgroundService
{
    protected KafkaSettings KafkaSettings;
    protected OrderExecutionSettings OrderExecutionSettings;
    protected IOrderEventRepository OrderEventRepository;
    protected IOperationsPublisher OperationsPublisher;
    protected ILogger Logger;

    protected abstract bool IsEnabled { get; }

    public ConsumerServiceBase(
        IOptions<KafkaSettings> kafkaSettings,
        IOptions<OrderExecutionSettings> orderExecutionSettings,
        IOrderEventRepository orderEventRepository,
        IOperationsPublisher operationsPublisher,
        ILogger logger)
    {
        KafkaSettings = kafkaSettings.Value;
        OrderExecutionSettings = orderExecutionSettings.Value;
        Logger = logger;
        OrderEventRepository = orderEventRepository;
        OperationsPublisher = operationsPublisher;
    }
}
