using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Domain.Ports;

namespace Vertr.Adapters.DataAccess.Tests;

[TestFixture(Category = "database", Explicit = true)]
public class TinvestCandlesRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;

    private readonly IConfiguration _configuration;

    protected ITinvestCandlesRepository Repo => _serviceProvider.GetRequiredService<ITinvestCandlesRepository>();

    public TinvestCandlesRepositoryTests()
    {
        _configuration = InitConfiguration();

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
        var from = to.AddDays(-10);

        var candles = await Repo.GetCandles("SBER", Domain.CandleInterval.Min10, from, to);

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
        var from = to.AddDays(-10);

        var candles = await Repo.GetLastCandles("SBER", Domain.CandleInterval.Min10, count, completedOnly);
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

    private static IConfiguration InitConfiguration()
    {
        var config = new ConfigurationBuilder()
           .AddJsonFile("appsettings.test.json")
           .AddEnvironmentVariables()
           .Build();

        return config;
    }
}
