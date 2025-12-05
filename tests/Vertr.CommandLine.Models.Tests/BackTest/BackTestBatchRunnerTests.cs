using Vertr.CommandLine.Models.BackTest;
using Vertr.CommandLine.Models.Helpers;
using Vertr.CommandLine.Models.Requests.Predictor;

namespace Vertr.CommandLine.Models.Tests.BackTest;

public class BackTestBatchRunnerTests : SystemTestBase
{
    private static readonly FileDataSource[] DataSources =
    [
        new FileDataSource
        {
            Symbol = "SBER",
            FilePath = "Data\\SBER_251001_251109.csv",
        }
    ];

    private static readonly BackTestParams BackTestParams =
        new BackTestParams
        {
            PortfolioId = Guid.NewGuid(),
            Symbol = "SBER",
            CurrencyCode = "RUB",
            Steps = 200,
            Skip = 10, // Ignored in batch run
            OpenPositionQty = 100,
            ComissionPercent = 0.0005m,
            PriceThreshold = 0.0001m,
            Intraday = null, // Ignored in batchByDay run
            Predictor = PredictorType.HistoricAverage,
            PredictionRequestCandlesCount = 100,
        };

    [Test]
    public async Task CanRunBackTestBatch()
    {
        var bt = new BackTestRunner(MarketDataService, PortfolioService, Mediator, Logger);
        await bt.InitMarketData(DataSources);
        var res = await bt.RunBatch(BackTestParams, 10);
        var summaries = res.Select(r => r.GetSummary(BackTestParams.CurrencyCode));

        var avgTrading = summaries.Average(s => s.TradingAmount);
        var avgComissions = summaries.Average(s => s.Comissions);
        var avgTotal = summaries.Average(s => s.TotalAmount);

        Console.WriteLine($"AVG: Trading={avgTrading:c} Comissions={avgComissions:c} Total={avgTotal:c}");
        Assert.Pass();
    }

    [Test]
    public async Task CanRunBackTestBatchByDay()
    {
        var bt = new BackTestRunner(MarketDataService, PortfolioService, Mediator, Logger);
        await bt.InitMarketData(DataSources);

        var res = await bt.RunBatchByDay(BackTestParams, 10);

        var sumTrading = 0m;
        var sumComissions = 0m;
        var sumTotal = 0m;

        foreach (var kvp in res)
        {
            var summaries = kvp.Value.Select(r => r.GetSummary(BackTestParams.CurrencyCode));
            var avgTrading = summaries.Average(s => s.TradingAmount);
            var avgComissions = summaries.Average(s => s.Comissions);
            var avgTotal = summaries.Average(s => s.TotalAmount);

            sumTrading += avgTrading;
            sumComissions += avgComissions;
            sumTotal += avgTotal;

            Console.WriteLine($"Day={kvp.Key} AVG: Trading={avgTrading:c} Comissions={avgComissions:c} Total={avgTotal:c}");
        }

        Console.WriteLine("\n----------------------------------------------");
        Console.WriteLine($"SUM: Trading={sumTrading:c} Comissions={sumComissions:c} Total={sumTotal:c}");

        Assert.Pass();
    }
}