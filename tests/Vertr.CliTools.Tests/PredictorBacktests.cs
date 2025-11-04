using Vertr.Backtest.Contracts;
using Vertr.CliTools.Tests.Model;
using Vertr.MarketData.Contracts.Extensions;

namespace Vertr.CliTools.Tests;

[TestFixture(Category = "System", Explicit = true)]
public class PredictorBacktests
{
    private const string _csvFilePath = "Data\\SBER_251101_251104.csv";

    [Test]
    public void CanCreateCandlesFromCsv()
    {
        var candels = CandleExtensions.FromCsv(_csvFilePath, Guid.Empty);
        Assert.That(candels, Is.Not.Empty);
    }

    [Test]
    public async Task CanRunBcktest()
    {
        var btRun = new BacktestRun()
        {
            Id = Guid.NewGuid(),
            PortfolioId = Guid.NewGuid(),
        };

        var strategy = new SimpleStrategy();
        var candels = CandleExtensions.FromCsv(_csvFilePath, strategy.InstrumentId);

        await strategy.OnStart(btRun);

        foreach (var candle in candels)
        {
            await strategy.HandleMarketData(candle);
        }

        await strategy.OnStop();

        Assert.Pass();
    }
}
