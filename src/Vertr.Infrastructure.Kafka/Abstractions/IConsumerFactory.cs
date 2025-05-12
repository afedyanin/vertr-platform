using Confluent.Kafka;

namespace Vertr.Infrastructure.Kafka.Abstractions;

public interface IConsumerFactory
{
    ConsumerConfig ConsumerConfig { get; }

    IConsumer<TKey, TValue> CreateConsumer<TKey, TValue>(bool readFromBegining = false);
}

