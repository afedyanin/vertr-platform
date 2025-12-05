using Vertr.CommandLine.Models.BackTest;
using Vertr.CommandLine.Models.Helpers;
using Vertr.CommandLine.Models.Requests.Predictor;

namespace Vertr.CommandLine.Models.Tests.BackTest;

public class BackTestRunnerTests : SystemTestBase
{
    private static readonly FileDataSource[] DataSources =
    [
        new FileDataSource
        {
            Symbol = "SBER",
            FilePath = "Data\\SBER_251101_251104.csv",
        }
    ];

    private static readonly BackTestParams BackTestParams =
        new BackTestParams
        {
            PortfolioId = Guid.NewGuid(),
            Symbol = "SBER",
            CurrencyCode = "RUB",
            Steps = 200,
            Skip = 0,
            OpenPositionQty = 100,
            ComissionPercent = 0.001m,
            Predictor = PredictorType.RandomWalkWithDrift,
            PredictionRequestCandlesCount = 100,
        };

    [Test]
    public async Task CanIterateBackTestSteps()
    {
        var candles = CsvImporter.LoadCandles(DataSources[0].FilePath);
        Assert.That(candles, Is.Not.Null);
        await MarketDataService.LoadData(BackTestParams.Symbol, [.. candles]);

        var candleRange = await MarketDataService.GetCandleRange(BackTestParams.Symbol);
        Assert.That(candleRange, Is.Not.Null);
        Console.WriteLine($"CandleRange={candleRange}");

        var stepCount = 0;
        var closeTime = candleRange.LastDate;
        var maxSteps = BackTestParams.Steps > 0 ? Math.Min(BackTestParams.Steps + BackTestParams.Skip, candleRange.Count) :
            candleRange.Count;

        var timeIndex = await MarketDataService.GetEnumerable(BackTestParams.Symbol);
        Assert.That(timeIndex, Is.Not.Null);

        foreach (var timeStep in timeIndex)
        {
            if (stepCount++ >= maxSteps)
            {
                closeTime = timeStep;
                break;
            }

            if (stepCount <= BackTestParams.Skip)
            {
                Console.WriteLine($"Skipping Step={stepCount} Time={timeStep:s}");
                continue;
            }

            Console.WriteLine($"Step={stepCount} Time={timeStep:s}");
        }

        Console.WriteLine($"CloseTime={closeTime:s}");

        Assert.Pass();
    }

    [Test]
    public async Task CanRunBackTest()
    {
        var bt = new BackTestRunner(MarketDataService, PortfolioService, Mediator, Logger);
        await bt.InitMarketData(DataSources);
        var res = await bt.Run(BackTestParams);

        DumpResults(res);
    }

    private static void DumpResults(BackTestResult backTestResult)
    {
        foreach (var result in backTestResult.DumpAll())
        {
            Console.WriteLine(result);
        }

        Console.WriteLine("\nLAST STEP:");
        Console.WriteLine(backTestResult.DumpLastStep());

        Console.WriteLine("\nCLOSE STEP:");
        Console.WriteLine(backTestResult.DumpCloseStep());

        Console.WriteLine("\nSUMMARY:");
        Console.WriteLine(backTestResult.GetSummary(BackTestParams.CurrencyCode));
    }
}