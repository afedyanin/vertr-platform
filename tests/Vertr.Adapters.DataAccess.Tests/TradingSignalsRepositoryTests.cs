using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Domain.Ports;
using Vertr.Domain;
using Vertr.Domain.Enums;

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
    public async Task CanInsertTradingSignals()
    {
        var signal = new TradingSignal
        {
            Id = Guid.NewGuid(),
            Symbol = "SBER",
            CandleInterval = CandleInterval._10Min,
            CandlesSource = "tinvest",
            PredictorType = PredictorType.Sb3,
            Sb3Algo = Sb3Algo.SAC,
            Action = TradeAction.Sell,
            TimeUtc = DateTime.UtcNow,
        };

        var res = await Repo.Insert(signal);
        Assert.That(res, Is.GreaterThan(0));
    }

    [Test]
    public async Task CanGetTradingSignals()
    {
        var to = DateTime.UtcNow.AddDays(1);
        var from = to.AddDays(-100);

        var signals = await Repo.Get("SBER", CandleInterval._10Min, from, to);

        Assert.That(signals, Is.Not.Null);
        Assert.That(signals.Count(), Is.GreaterThan(0));

        foreach (var signal in signals)
        {
            Console.WriteLine(signal);
        }
    }

    [Test]
    public async Task CanGetLastSignal()
    {
        var signals = await Repo.GetLast("SBER", CandleInterval._10Min, 1);

        Assert.That(signals, Is.Not.Null);
        Assert.That(signals.Count(), Is.EqualTo(1));

        Console.WriteLine(signals.Single());
    }

    [Test]
    public async Task CanDeleteSignal()
    {
        var signals = await Repo.GetLast("SBER", CandleInterval._10Min, 1);
        var deleted = await Repo.Delete(signals.Single().Id);
        Assert.That(deleted, Is.EqualTo(1));
    }
}
