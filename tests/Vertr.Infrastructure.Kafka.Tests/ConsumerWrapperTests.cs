using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Infrastructure.Kafka.Abstractions;

namespace Vertr.Infrastructure.Kafka.Tests;

[TestFixture(Category = "System", Explicit = true)]
public class ConsumerWrapperTests : KafkaTestBase
{
    [Test]
    public void CanCreateConsumerFactory()
    {
        var consumerFactory = ServiceProvider.GetRequiredService<IConsumerFactory>();
        Assert.That(consumerFactory, Is.Not.Null);
    }

    [Test]
    public void CanCreateConsumerWrapper()
    {
        var consumer = ServiceProvider.GetRequiredService<IConsumerWrapper<string, string>>();
        Assert.That(consumer, Is.Not.Null);
    }

    [Test]
    public async Task CanStartConsumerWrapper()
    {
        var consumer = ServiceProvider.GetRequiredService<IConsumerWrapper<string, string>>();
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await consumer.Consume([StringTopic], Handle, true, cts.Token);
        Assert.Pass();
    }

    [Test]
    public async Task CanConsumeTypedItems()
    {
        var consumer = ServiceProvider.GetRequiredService<IConsumerWrapper<Guid, SimpleEntity>>();
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await consumer.Consume([TypedTopic], HandleTypedItem, true, cts.Token);
        Assert.Pass();
    }


    private Task Handle(ConsumeResult<string, string> result, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Handle result: Key={result.Message.Key} Value={result.Message.Value}");
        return Task.CompletedTask;
    }

    private Task HandleTypedItem(ConsumeResult<Guid, SimpleEntity> result, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Handle typed result: Key={result.Message.Key} Value={result.Message.Value}");
        return Task.CompletedTask;
    }
}
