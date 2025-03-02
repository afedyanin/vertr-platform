using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Domain;
using Vertr.Domain.Enums;
using Vertr.Domain.Repositories;
using Vertr.Domain.Settings;

namespace Vertr.Adapters.DataAccess.Tests.Repositories;

[TestFixture(Category = "database", Explicit = true)]
public class TradingSignalsRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    private readonly StrategySettings _strategySettings = new StrategySettings
    {
        Symbol = "SBER",
        Interval = CandleInterval._10Min,
        PredictorType = PredictorType.Sb3,
        Sb3Algo = Sb3Algo.DQN,
    };

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
            Symbol = _strategySettings.Symbol,
            CandleInterval = _strategySettings.Interval,
            CandlesSource = "tinvest",
            PredictorType = _strategySettings.PredictorType,
            Sb3Algo = _strategySettings.Sb3Algo,
            Action = TradeAction.Sell,
            TimeUtc = DateTime.UtcNow,
        };

        var res = await Repo.Insert(signal);
        Assert.That(res, Is.GreaterThan(0));
    }

    [Test]
    public async Task CanGetLastSignal()
    {
        var signal = await Repo.GetLast(_strategySettings);
        Assert.That(signal, Is.Not.Null);
        Console.WriteLine(signal);
    }

    [Test]
    public async Task CanDeleteSignal()
    {
        var signal = await Repo.GetLast(_strategySettings);
        Assert.That(signal, Is.Not.Null);
        var deleted = await Repo.Delete(signal.Id);
        Assert.That(deleted, Is.EqualTo(1));
    }
}
