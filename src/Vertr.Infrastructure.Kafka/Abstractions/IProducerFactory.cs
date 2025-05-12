using Confluent.Kafka;

namespace Vertr.Infrastructure.Kafka.Abstractions;
public interface IProducerFactory
{
    ProducerConfig ProducerConfig { get; }

    IProducer<TKey, TValue> CreateProducer<TKey, TValue>();
}
