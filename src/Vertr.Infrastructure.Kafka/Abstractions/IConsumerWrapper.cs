using Confluent.Kafka;

namespace Vertr.Infrastructure.Kafka.Abstractions;
public interface IConsumerWrapper<TKey, TValue>
{
    Task Consume(
        string[] topics,
        Func<ConsumeResult<TKey, TValue>, CancellationToken, Task> handler,
        bool readFromBegining = false,
        CancellationToken stoppingToken = default);
}
