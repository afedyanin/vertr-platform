using System.Text.Json;
using Confluent.Kafka;

namespace Vertr.Infrastructure.Kafka;
public class KafkaSettings
{
    public ConsumerConfig ConsumerSettings { get; set; } = new ConsumerConfig();

    public ProducerConfig ProducerSettings { get; set; } = new ProducerConfig();

    public IDictionary<string, string> Topics { get; set; } = new Dictionary<string, string>();

    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions();
}
