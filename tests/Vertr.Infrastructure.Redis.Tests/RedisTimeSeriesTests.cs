using NetTopologySuite.Index.HPRtree;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

namespace Vertr.Infrastructure.Redis.Tests;

public class RedisTimeSeriesTests
{
    private record TsEntry(DateTime Time, double Price);

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
    public async Task CanUseTimeSeries()
    {
        var ts = _redis!.GetDatabase().TS();

        await ts.CreateAsync("test3", new TsCreateParamsBuilder().build());

        var addParams = new TsAddParamsBuilder()
        .AddTimestamp(new NRedisStack.DataTypes.TimeStamp("*"))
        .AddValue(0.2)
        .AddOnDuplicate(NRedisStack.Literals.Enums.TsDuplicatePolicy.LAST)
        .build();

        await ts.AddAsync("test3", addParams);
        var res = await ts.GetAsync("test3");
        DateTime time = res!.Time;
        Console.WriteLine($"time={time:O} value={res.Val}");
    }

    [Test]
    public async Task CanSaveSeries()
    {
        const string key = "test-series";
        var db = _redis!.GetDatabase();
        var ts = db.TS();

        await ts.CreateAsync(key, new TsCreateParamsBuilder().build());

        var builder = new TsAddParamsBuilder();
        builder.AddOnDuplicate(NRedisStack.Literals.Enums.TsDuplicatePolicy.LAST);

        foreach (var item in GenerateSeries(DateTime.UtcNow))
        {
            var addParams = builder
            .AddTimestamp(new NRedisStack.DataTypes.TimeStamp(item.Time))
            .AddValue((double)item.Price)
            .build();

            await ts.AddAsync(key, addParams);
        }

        var info = await ts.InfoAsync(key);
        Console.WriteLine($"first={info.FirstTimeStamp!.Value.ToString()} last={info.LastTimeStamp!.Value.ToString()} TotalSamples={info.TotalSamples}");

        await db.KeyDeleteAsync(key);
    }

    [Test]
    public async Task CanGetRange()
    {
        const string key = "test-series";
        var db = _redis!.GetDatabase();
        var ts = db.TS();

        await ts.CreateAsync(key, new TsCreateParamsBuilder().build());

        var builder = new TsAddParamsBuilder();
        builder.AddOnDuplicate(NRedisStack.Literals.Enums.TsDuplicatePolicy.LAST);

        var tStart = DateTime.UtcNow.AddHours(-1);

        foreach (var item in GenerateSeries(tStart))
        {
            var addParams = builder
            .AddTimestamp(new NRedisStack.DataTypes.TimeStamp(item.Time))
            .AddValue((double)item.Price)
            .build();

            await ts.AddAsync(key, addParams);
        }

        var rangeFrom = tStart.AddHours(-20);
        var rangeTo = tStart.AddHours(20);

        var range1 = await ts.RangeAsync(key, rangeFrom, rangeTo);
        var count1 = 0;
        foreach (var item in range1)
        {
            DateTime time = item.Time;
            Console.WriteLine($"{count1++}: {time:O} {item.Val}");
        }

        Console.WriteLine("Range2");
        var range2 = await ts.RangeAsync(key, rangeFrom, rangeTo, count: 20);
        var count2 = 0;
        foreach (var item in range2)
        {
            DateTime time = item.Time;
            Console.WriteLine($"{count2++}: {time:O} {item.Val}");
        }

        await db.KeyDeleteAsync(key);
    }

    [Test]
    public async Task CanGetRevRange()
    {
        const string key = "test-series";
        var db = _redis!.GetDatabase();
        var ts = db.TS();

        await ts.CreateAsync(key, new TsCreateParamsBuilder().build());

        var builder = new TsAddParamsBuilder();
        builder.AddOnDuplicate(NRedisStack.Literals.Enums.TsDuplicatePolicy.LAST);

        var tStart = DateTime.UtcNow.AddHours(-1);

        foreach (var item in GenerateSeries(tStart))
        {
            var addParams = builder
            .AddTimestamp(new NRedisStack.DataTypes.TimeStamp(item.Time))
            .AddValue((double)item.Price)
            .build();

            await ts.AddAsync(key, addParams);
        }

        var rangeFrom = tStart.AddHours(-20);
        var rangeTo = tStart.AddHours(20);

        var range1 = await ts.RevRangeAsync(key, rangeFrom, rangeTo);
        var count1 = 0;
        foreach (var item in range1)
        {
            DateTime time = item.Time;
            Console.WriteLine($"{count1++}: {time:O} {item.Val}");
        }

        Console.WriteLine("Range2");
        var range2 = await ts.RevRangeAsync(key, rangeFrom, rangeTo, count: 20);
        var count2 = 0;
        foreach (var item in range2)
        {
            DateTime time = item.Time;
            Console.WriteLine($"{count2++}: {time:O} {item.Val}");
        }

        await db.KeyDeleteAsync(key);
    }

    private TsEntry[] GenerateSeries(DateTime startTime, long size = 100)
    {
        var items = new TsEntry[size];
        var time = startTime;
        for (int i = 0; i < size; i++)
        {
            items[i] = new TsEntry(time, Random.Shared.NextDouble());
            time = time.AddSeconds(1);
        }

        return items;
    }
}
