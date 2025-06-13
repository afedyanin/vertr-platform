using StackExchange.Redis;

namespace Vertr.Infrastructure.Redis.Tests;

public class RedisConnectionTests
{
    private ConnectionMultiplexer? _redis;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _redis = ConnectionMultiplexer.Connect("localhost");
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        if (_redis != null)
        {
            _redis.Dispose();
            _redis = null;
        }
    }

    [Test]
    public void CanConnectToRedis()
    {
        var db = _redis!.GetDatabase();
        db.StringSet("foo", "bar");
        var value = db.StringGet("foo");

        Assert.That(value.ToString(), Is.EqualTo("bar"));
    }

    [Test]
    public void CanUseHashSet()
    {
        var hash = new HashEntry[] {
            new HashEntry("name", "John"),
            new HashEntry("surname", "Smith"),
            new HashEntry("company", "Redis"),
            new HashEntry("age", "29"),
        };

        var db = _redis!.GetDatabase();
        db.HashSet("user-session:123", hash);

        var hashFields = db.HashGetAll("user-session:123");

        Assert.Multiple(() =>
        {
            Assert.That(hashFields.Single(h => h.Name == "name").Value.ToString(), Is.EqualTo("John"));
            Assert.That(hashFields.Single(h => h.Name == "surname").Value.ToString(), Is.EqualTo("Smith"));
            Assert.That(hashFields.Single(h => h.Name == "company").Value.ToString(), Is.EqualTo("Redis"));
            Assert.That(hashFields.Single(h => h.Name == "age").Value.ToString(), Is.EqualTo("29"));
        });
    }

    [Test]
    public async Task CanUsePubSub()
    {
        var cts = new CancellationTokenSource(5000);
        var t2 = DoSubscribe(cts.Token);
        var t1 = DoPublish(cts.Token);

        await Task.WhenAll(t1, t2);
    }


    private async Task DoPublish(CancellationToken cancellationToken)
    {
        var sub = _redis!.GetSubscriber();

        Console.WriteLine("Start publishing messages.");

        var count = 0;

        while (!cancellationToken.IsCancellationRequested)
        {
            await sub.PublishAsync("test_channel", $"Redis message {count++}");

            await Task.Delay(500);
        }

        Console.WriteLine("End publishing messages.");
    }

    private async Task DoSubscribe(CancellationToken cancellationToken)
    {
        var channel = await _redis!.GetSubscriber().SubscribeAsync("test_channel");

        Console.WriteLine("Start receiving messages.");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await channel.ReadAsync(cancellationToken);
                Console.WriteLine($"Message received: {message.Message}");
            }
        }
        catch (OperationCanceledException)
        {
        }

        Console.WriteLine("End receiving messages.");
    }
}
