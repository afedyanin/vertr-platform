using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Vertr.Infrastructure.Kafka.Abstractions;

namespace Vertr.Infrastructure.Kafka;
internal sealed class ProducerWrapper<TKey, TValue> : IProducerWrapper<TKey, TValue>
{
    private readonly IProducer<TKey, TValue> _producer;
    private readonly ILogger<ProducerWrapper<TKey, TValue>> _logger;

    public ProducerWrapper(
        IProducerFactory producerFactory,
        ILogger<ProducerWrapper<TKey, TValue>> logger)
    {
        _logger = logger;
        _producer = producerFactory.CreateProducer<TKey, TValue>();
    }

    public void Dispose()
    {
        _producer.Flush();
        _producer.Dispose();
    }

    public async Task<DeliveryResult<TKey, TValue>> Produce(
        string topic,
        TKey key,
        TValue value,
        DateTime? timestamp = null,
        CancellationToken cancellationToken = default)
    {
        var message = new Message<TKey, TValue>()
        {
            Key = key,
            Value = value,
            Timestamp = timestamp.HasValue ? new Timestamp(timestamp.Value) : new Timestamp(DateTime.UtcNow),
        };

        _logger.LogDebug($"Start producing message.");

        var res = await _producer.ProduceAsync(topic, message, cancellationToken);

        _logger.LogDebug($"Producing result: Status={res.Status}");

        return res;
    }
}
