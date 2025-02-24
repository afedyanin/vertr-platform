using Microsoft.Extensions.DependencyInjection;
using Vertr.Application.Signals;
using Vertr.Domain;
using Vertr.Domain.Ports;

namespace Vertr.Application.Tests.Signals;

[TestFixture(Category = "System", Explicit = true)]
public class GenerateSignalsHandlerTests : ApplicationTestBase
{
    private const string _symbol = "SBER";
    private const CandleInterval _interval = CandleInterval._10Min;
    private static readonly PredictorType _predictorType = PredictorType.Sb3;
    private static readonly Sb3Algo _algo = Sb3Algo.DQN;

    [Test]
    public async Task CanCheckIfNewSignalShouldBeGenerated()
    {
        var handler = ServiceProvider.GetRequiredService<GenerateSignalsHandler>();
        Assert.That(handler, Is.Not.Null);

        var shouldGenerate = await handler.ShouldGenerateNewSignal(_symbol, _interval);

        Assert.That(shouldGenerate, Is.True);
    }

    [Test]
    public async Task CanGenerateSignalForSymbol()
    {
        var handler = ServiceProvider.GetRequiredService<GenerateSignalsHandler>();

        await handler.HandleSymbol(_symbol, _interval, _predictorType, _algo, CancellationToken.None);

        // Check last signal with last candle in DB
        var candlesRepo = ServiceProvider.GetRequiredService<ITinvestCandlesRepository>();
        var signalsRepo = ServiceProvider.GetRequiredService<ITradingSignalsRepository>();

        var candles = await candlesRepo.GetLast(_symbol, _interval, 1, false);
        var signals = await signalsRepo.GetLast(_symbol, _interval, 1);

        var lastSignal = signals.Last();
        var lastCandle = candles.Last();

        Assert.Multiple(() =>
        {
            Assert.That(lastSignal.TimeUtc, Is.EqualTo(lastCandle.TimeUtc));
            Assert.That(lastSignal.PredictorType.Name, Is.EqualTo(_predictorType.Name));
            Assert.That(lastSignal.Sb3Algo.Name, Is.EqualTo(_algo.Name));
        });
    }
}
