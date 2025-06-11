using StackExchange.Redis;

namespace Vertr.Infrastructure.Redis.Tests;

public class RedisConnectionTests
{
    [Test]
    public void CanConnectToRedis()
    {
        var redis = ConnectionMultiplexer.Connect("localhost");
        var db = redis.GetDatabase();

        db.StringSet("foo", "bar");
        Console.WriteLine(db.StringGet("foo"));

        Assert.Pass();
    }
}
