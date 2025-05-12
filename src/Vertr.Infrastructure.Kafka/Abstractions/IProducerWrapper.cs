using Confluent.Kafka;

namespace Vertr.Infrastructure.Kafka.Abstractions;
public interface IProducerWrapper<TKey, TValue> : IDisposable
{
    Task<DeliveryResult<TKey, TValue>> Produce(
        string topic,
        TKey key,
        TValue value,
        DateTime? timestamp = null,
        CancellationToken cancellationToken = default);
}
