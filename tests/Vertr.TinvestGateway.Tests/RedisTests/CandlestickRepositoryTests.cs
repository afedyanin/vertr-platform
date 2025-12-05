using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.MarketData;
using Vertr.TinvestGateway.DataAccess.Redis;
using Vertr.TinvestGateway.Tests.Stubs;

namespace Vertr.TinvestGateway.Tests.RedisTests;

public class CandlestickRepositoryTests
{
    private readonly Guid _sberId = new Guid("e6123145-9665-43e0-8413-cd61b8aa9b13");

    private IConnectionMultiplexer _connectionMultiplexer;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _connectionMultiplexer = ConnectionMultiplexer.Connect("localhost");
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _connectionMultiplexer.Close();
    }

    [TearDown]
    public async Task TearDown()
    {
        var repo = new CandlestickRepository(_connectionMultiplexer);
        await repo.Clear(_sberId);
    }


    [Test]
    public async Task CanSaveCandles()
    {
        var repo = new CandlestickRepository(_connectionMultiplexer);
        var startTime = new DateTime(2025, 11, 24, 10, 3, 0);
        var items = GenerateCandles(startTime, 5, 290, 3);

        var savedCount = await repo.Save(_sberId, items);
        Assert.That(savedCount, Is.EqualTo(items.Length));
    }

    [Test]
    public async Task CanSaveCandlesWithOverride()
    {
        var repo = new CandlestickRepository(_connectionMultiplexer);

        var startTime = new DateTime(2025, 11, 24, 10, 3, 0);
        var items = GenerateCandles(startTime, 15, 290, 3);

        var savedCount = await repo.Save(_sberId, items, 5);
        Assert.That(savedCount, Is.EqualTo(5));

        var saved = await repo.GetLast(_sberId);

        Assert.That(saved.Count, Is.EqualTo(5));

        foreach (var item in saved)
        {
            Console.WriteLine($"Time={item!.Value.GetDateTime()} {item}");
        }
    }

    [Test]
    public async Task CanSaveCandlesWithOverride2()
    {
        var repo = new CandlestickRepository(_connectionMultiplexer);

        var startTime = new DateTime(2025, 11, 24, 10, 3, 0);
        var items = GenerateCandles(startTime, 15, 290, 3);

        var toAdd1 = items.OrderBy(c => c.Time).Take(7).ToArray();

        var savedCount1 = await repo.Save(_sberId, toAdd1, 5);
        Assert.That(savedCount1, Is.EqualTo(5));

        var saved1 = await repo.GetLast(_sberId);
        Console.WriteLine(" STEP 1 ===============================");
        foreach (var item in saved1)
        {
            Console.WriteLine($"Time={item!.Value.GetDateTime()} {item}");
        }

        var toAdd2 = items.OrderByDescending(c => c.Time).Take(3).ToArray();
        var savedCount2 = await repo.Save(_sberId, toAdd2, 5);
        Assert.That(savedCount2, Is.EqualTo(3));

        var saved2 = await repo.GetLast(_sberId);
        Console.WriteLine(" STEP 2 ===============================");
        foreach (var item in saved2)
        {
            Console.WriteLine($"Time={item!.Value.GetDateTime()} {item}");
        }

        Assert.Pass();
    }

    [Test]
    public async Task CanSaveCandlesWithOverride3()
    {
        var repo = new CandlestickRepository(_connectionMultiplexer);

        var startTime = new DateTime(2025, 11, 24, 10, 3, 0);
        var items = GenerateCandles(startTime, 5, 290, 3);

        _ = await repo.Save(_sberId, items, 5);
        var saved1 = await repo.GetLast(_sberId);
        Console.WriteLine(" STEP 1: Заполняем буфер в 5 свечей");
        foreach (var item in saved1)
        {
            Console.WriteLine($"Time={item!.Value.GetDateTime()} {item}");
        }

        var last = items.OrderBy(c => c.Time).Last();

        Console.WriteLine(" STEP 2: Генерим новую свечу и сохраняем в буфер с лимитом в 5. Ожидаем что, новая свеча вытеснит самую старую");
        var next1 = GenerateCandles(last.GetDateTime().AddMinutes(10), 1, 300, 7);
        _ = await repo.Save(_sberId, next1, 5);
        var saved2 = await repo.GetLast(_sberId);

        foreach (var item in saved2)
        {
            Console.WriteLine($"Time={item!.Value.GetDateTime()} {item}");
        }

        Console.WriteLine(" STEP 3: Ожидаем что, вторая новая свеча вытеснит еще одну самую старую");
        var next2 = GenerateCandles(last.GetDateTime().AddMinutes(15), 1, 300, 7);
        _ = await repo.Save(_sberId, next2, 5);
        var saved3 = await repo.GetLast(_sberId);
        foreach (var item in saved3)
        {
            Console.WriteLine($"Time={item!.Value.GetDateTime()} {item}");
        }

        Assert.Pass();
    }

    [Test]
    public async Task CanGetCandles()
    {
        var repo = new CandlestickRepository(_connectionMultiplexer);
        var startTime = new DateTime(2025, 11, 24, 10, 3, 0);
        var items = GenerateCandles(startTime, 5, 290, 3);
        var savedCount = await repo.Save(_sberId, items);

        var last = await repo.GetLast(_sberId);

        Assert.That(last.Count, Is.GreaterThan(0));

        foreach (var item in last)
        {
            Console.WriteLine($"Time={item!.Value.GetDateTime()} {item}");
        }

        Assert.Pass();
    }

    private static Candlestick[] GenerateCandles(DateTime startTime, int count, decimal mean, decimal stdDev)
    {
        var nr = new NormalRandom((double)mean, (double)stdDev);

        var res = new List<Candlestick>();
        var time = startTime;

        for (var i = 0; i < count; i++)
        {
            var candle = new Candlestick(
                time,
                (decimal)nr.NextDouble(),
                (decimal)nr.NextDouble(),
                (decimal)nr.NextDouble(),
                (decimal)nr.NextDouble(),
                (decimal)nr.NextDouble() * 10);

            time = time.AddMinutes(1);
            res.Add(candle);
        }

        return res.ToArray();
    }
}