using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Vertr.Infrastructure.Kafka.Abstractions;

namespace Vertr.Infrastructure.Kafka;
internal sealed class ProducerFactory : IProducerFactory
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public ProducerConfig ProducerConfig { get; private set; }

    public ProducerFactory(IOptions<KafkaSettings> kafkaOptions)
    {
        ProducerConfig = kafkaOptions.Value.ProducerSettings;
        _jsonSerializerOptions = kafkaOptions.Value.JsonSerializerOptions ?? new JsonSerializerOptions();
    }

    public IProducer<TKey, TValue> CreateProducer<TKey, TValue>()
    {
        var producerBuilder = new ProducerBuilder<TKey, TValue>(ProducerConfig);

        var keySerializer = new DefaultJsonSerializer<TKey>(_jsonSerializerOptions);
        var valueSerializer = new DefaultJsonSerializer<TValue>(_jsonSerializerOptions);

        producerBuilder
            .SetKeySerializer(keySerializer)
            .SetValueSerializer(valueSerializer);

        var producer = producerBuilder.Build();
        return producer;
    }
}
