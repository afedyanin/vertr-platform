using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Infrastructure.Kafka.Abstractions;

namespace Vertr.Infrastructure.Kafka.Tests;

[TestFixture(Category = "System", Explicit = true)]
public class ProducerWrapperTests : KafkaTestBase
{
    [Test]
    public void CanCreateProducerFactory()
    {
        var factory = ServiceProvider.GetRequiredService<IProducerFactory>();
        Assert.That(factory, Is.Not.Null);
    }

    [Test]
    public void CanCreateProducer()
    {
        var producer = ServiceProvider.GetRequiredService<IProducerWrapper<string, string>>();
        Assert.That(producer, Is.Not.Null);
    }

    [Test]
    public async Task CanProduceMessage()
    {
        using (var producer = ServiceProvider.GetRequiredService<IProducerWrapper<string, string>>())
        {
            var res = await producer.Produce(StringTopic, "key", "Hello!");

            Assert.That(res.Status, Is.EqualTo(PersistenceStatus.Persisted));
        }
    }

    [Test]
    public async Task CanProduceTypesMessages()
    {
        using (var producer = ServiceProvider.GetRequiredService<IProducerWrapper<Guid, SimpleEntity>>())
        {
            for (int i = 0; i < 10; i++)
            {
                var item = SimpleEntity.Create(i);
                var res = await producer.Produce(TypedTopic, item.Id, item);

                Assert.That(res.Status, Is.EqualTo(PersistenceStatus.Persisted));
            }
        }
    }

}
