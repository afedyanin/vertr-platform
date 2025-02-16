using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Domain.Ports;
using Vertr.Domain;

namespace Vertr.Adapters.DataAccess.Tests;

[TestFixture(Category = "database", Explicit = true)]
public class TradingSignalsRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;

    private readonly IConfiguration _configuration;

    protected ITradingSignalsRepository Repo => _serviceProvider.GetRequiredService<ITradingSignalsRepository>();

    public TradingSignalsRepositoryTests()
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
    public async Task CanGetTradingSignals()
    {
        var to = DateTime.UtcNow;
        var from = to.AddDays(-100);

        var candles = await Repo.Get("SBER", CandleInterval.Min10, from, to);

        Assert.That(candles, Is.Not.Null);
        Assert.That(candles.Count(), Is.GreaterThan(1));

        foreach (var candle in candles)
        {
            Console.WriteLine(candle);
        }
    }
}
