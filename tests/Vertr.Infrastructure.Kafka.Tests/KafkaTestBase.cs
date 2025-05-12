using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Vertr.Infrastructure.Kafka.Tests;
public abstract class KafkaTestBase
{
    protected static readonly string StringTopic = "Topic1";
    protected static readonly string TypedTopic = "Topic2";

    protected ServiceProvider ServiceProvider { get; private set; }

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        var services = new ServiceCollection();

        var kafkaSettings = new KafkaSettings();

        var jsonSettings = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        };

        kafkaSettings.ConsumerSettings.GroupId = "TestGroupId";
        kafkaSettings.ConsumerSettings.BootstrapServers = "localhost:9092";
        kafkaSettings.ConsumerSettings.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;

        kafkaSettings.ProducerSettings.BootstrapServers = "localhost:9092";

        services.AddKafkaSettings(settings =>
        {
            settings.ConsumerSettings = kafkaSettings.ConsumerSettings;
            settings.ProducerSettings = kafkaSettings.ProducerSettings;
            settings.Topics = kafkaSettings.Topics;
            settings.JsonSerializerOptions = jsonSettings;
        });

        services.AddKafkaProducer<string, string>();
        services.AddKafkaConsumer<string, string>();

        services.AddKafkaProducer<Guid, SimpleEntity>();
        services.AddKafkaConsumer<Guid, SimpleEntity>();

        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        ServiceProvider = services.BuildServiceProvider();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        ServiceProvider.Dispose();
    }
}
