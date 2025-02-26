using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Domain;
using Vertr.Domain.Enums;
using Vertr.Domain.Repositories;

namespace Vertr.Adapters.DataAccess.Tests;

[TestFixture(Category = "database", Explicit = true)]
public class TinvestCandlesRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;

    private readonly IConfiguration _configuration;

    protected ITinvestCandlesRepository Repo => _serviceProvider.GetRequiredService<ITinvestCandlesRepository>();

    public TinvestCandlesRepositoryTests()
    {
        _configuration = ConfigFactory.InitConfiguration();

        var services = new ServiceCollection();
        services.AddDataAccess(_configuration);

        _serviceProvider = services.BuildServiceProvider();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _serviceProvider.Dispose();
    }

    [Test]
    public async Task CanGetCandles()
    {
        var to = DateTime.UtcNow;
        var from = to.AddDays(-100);

        var candles = await Repo.Get("SBER", CandleInterval._10Min, from, to);

        Assert.That(candles, Is.Not.Null);
        Assert.That(candles.Count(), Is.GreaterThan(1));

        foreach (var candle in candles)
        {
            Console.WriteLine(candle);
        }
    }

    [TestCase(true, 10)]
    [TestCase(false, 10)]
    public async Task CanGetLastCandles(bool completedOnly, int count)
    {
        var to = DateTime.UtcNow;
        var from = to.AddDays(-100);

        var candles = await Repo.GetLast("SBER", CandleInterval._10Min, count, completedOnly);
        var hasNonCompletedCandles = candles.Any(c => !c.IsCompleted);

        Assert.That(candles, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(candles.Count(), Is.EqualTo(10));
            Assert.That(!hasNonCompletedCandles, Is.EqualTo(completedOnly));
        });


        foreach (var candle in candles)
        {
            Console.WriteLine(candle);
        }
    }

    [Test]
    public async Task CanDeleteCandles()
    {
        var to = new DateTime(2025, 02, 12, 11, 10, 0, DateTimeKind.Utc);
        var from = to.AddDays(-100);

        var deletedCount = await Repo.Delete("TEST", CandleInterval._10Min, from, to);

        Assert.That(deletedCount, Is.GreaterThan(1));
    }

    [Test]
    public async Task CanInsertCandles()
    {
        var startDate = new DateTime(2025, 02, 10, 6, 0, 0, DateTimeKind.Utc);
        var candels = GenerateCandles(startDate, 10);

        var inserted = await Repo.Insert("TEST", CandleInterval._10Min, candels);

        Assert.That(inserted, Is.EqualTo(10));
    }

    private static IEnumerable<HistoricCandle> GenerateCandles(DateTime startDate, int count, int incMinutes = 10)
    {
        for (int i = 0; i < count; i++)
        {
            yield return new HistoricCandle
            {
                CandleSource = 0,
                IsCompleted = true,
                TimeUtc = startDate.AddMinutes(incMinutes * i),
                Open = 213.13m,
                Close = 215.34m,
                High = 234.56m,
                Low = 206.78m,
                Volume = 12345,
            };
        }
    }
}
