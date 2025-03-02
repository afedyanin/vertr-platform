using Microsoft.Extensions.DependencyInjection;
using Vertr.Application.Signals;
using Vertr.Domain.Enums;
using Vertr.Domain.Repositories;
using Vertr.Domain.Settings;

namespace Vertr.Application.Tests.Signals;

[TestFixture(Category = "System", Explicit = true)]
public class GenerateSignalsHandlerTests : ApplicationTestBase
{
    private readonly StrategySettings _strategySettings = new StrategySettings
    {
        Symbol = "SBER",
        Interval = CandleInterval._10Min,
        PredictorType = PredictorType.Sb3,
        Sb3Algo = Sb3Algo.DQN,
    };

    [Test]
    public async Task CanCheckIfNewSignalShouldBeGenerated()
    {
        var handler = ServiceProvider.GetRequiredService<GenerateSignalsHandler>();
        Assert.That(handler, Is.Not.Null);

        var shouldGenerate = await handler.ShouldGenerateNewSignal(_strategySettings, CancellationToken.None);

        Assert.That(shouldGenerate, Is.True);
    }

    [Test]
    public async Task CanGenerateSignalForSymbol()
    {
        var handler = ServiceProvider.GetRequiredService<GenerateSignalsHandler>();

        await handler.HandleStrategy(_strategySettings, CancellationToken.None);

        // Check last signal with last candle in DB
        var candlesRepo = ServiceProvider.GetRequiredService<ITinvestCandlesRepository>();
        var signalsRepo = ServiceProvider.GetRequiredService<ITradingSignalsRepository>();

        var candles = await candlesRepo.GetLast(
            _strategySettings.Symbol,
            _strategySettings.Interval,
            1,
            false);

        var lastSignal = await signalsRepo.GetLast(_strategySettings);

        var lastCandle = candles.Last();

        Assert.Multiple(() =>
        {
            Assert.That(lastSignal, Is.Not.Null);
            Assert.That(lastSignal.TimeUtc, Is.EqualTo(lastCandle.TimeUtc));
            Assert.That(lastSignal.PredictorType, Is.EqualTo(_strategySettings.PredictorType));
            Assert.That(lastSignal.Sb3Algo, Is.EqualTo(_strategySettings.Sb3Algo));
        });
    }
}
