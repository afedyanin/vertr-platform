using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.MarketData;

namespace Vertr.TinvestGateway.Tests.RedisTests;

// https://stackexchange.github.io/StackExchange.Redis/Basics
public class SimpleRedisTests
{
    [Test]
    public async Task CanConnectToRedis()
    {
        var redis = await ConnectionMultiplexer.ConnectAsync("localhost");
        var db = redis.GetDatabase();

        var value = "abcdefg";
        await db.StringSetAsync("mykey", value);

        var saved = await db.StringGetAsync("mykey");
        Console.WriteLine(saved);

        Assert.Pass();
    }

    [Test]
    public async Task CanUsePubSub()
    {
        var redis = await ConnectionMultiplexer.ConnectAsync("localhost");
        var sub = redis.GetSubscriber();

        var cahnnel = new RedisChannel("messages", RedisChannel.PatternMode.Literal);
        var queue = await sub.SubscribeAsync(cahnnel);

        queue.OnMessage(async channelMessage =>
        {
            await Task.Delay(1000);
            Console.WriteLine(channelMessage.Message);
        });

        await sub.PublishAsync(cahnnel, "hello");
        await Task.Delay(5000);

        Assert.Pass();
    }

    [Test]
    public async Task CanUseSortedSet()
    {
        var redis = await ConnectionMultiplexer.ConnectAsync("localhost");
        var db = redis.GetDatabase();

        var key = new RedisKey("sber");

        var t1 = new DateTime(2025, 11, 24, 10, 1, 0);
        var t2 = new DateTime(2025, 11, 24, 10, 2, 0);
        // var t3 = new DateTime(2025, 11, 24, 10, 3, 0);
        var t4 = new DateTime(2025, 11, 24, 10, 4, 0);

        // Cleanup
        _ = await db.SortedSetRemoveRangeByScoreAsync(key, t1.Ticks, t4.Ticks);

        var cs1 = new Candlestick(t1, 126.56m, 117.13m, 156.456m, 99.01m, 834);
        var e1 = new SortedSetEntry(cs1.ToJson(), t1.Ticks);

        var cs2 = new Candlestick(t2, 189.56m, 117.34m, 156m, 99m, 834m);
        var e2 = new SortedSetEntry(cs2.ToJson(), t2.Ticks);

        _ = await db.SortedSetAddAsync(key, [e1, e2], SortedSetWhen.Always);

        var saved = await db.SortedSetRangeByRankWithScoresAsync(key, 0, 10, Order.Descending);

        foreach (var value in saved)
        {
            var time = new DateTime((long)value.Score);
            var item = Candlestick.FromJson(value.Element.ToString());
            Console.WriteLine($"Time={time:O} {item}");
        }

        Assert.Pass();
    }


    [Test]
    public async Task CanUseHashSet()
    {
        var redis = await ConnectionMultiplexer.ConnectAsync("localhost");
        var db = redis.GetDatabase();

        var key = new RedisKey("market.instruments");

        var id = Guid.NewGuid();
        var instrument = new Instrument
        {
            ClassCode = "ClassCode",
            Ticker = "Ticker",
            Currency = "RUB",
            Id = id,
            InstrumentType = "SomeType",
            LotSize = 10,
            Name = "Some instrument name"
        };

        var item = new HashEntry(instrument.Id.ToString(), instrument.ToJson());

        await db.HashSetAsync(key, [item]);
        var saved = await db.HashGetAsync(key, instrument.Id.ToString());
        var restored = Instrument.FromJson(saved.ToString());

        Assert.That(restored, Is.Not.Null);

        Console.WriteLine(restored);
    }
}