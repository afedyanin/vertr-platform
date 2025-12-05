using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.BackTest;
using Vertr.CommandLine.Models.Helpers;
using Vertr.CommandLine.Models.Requests.BackTest;
using Vertr.CommandLine.Models.Requests.Predictor;

namespace Vertr.CommandLine.Application.Tests.Handlers;

public class BackTestExecuteStepHandlerTests : AppliactionTestBase
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
            Steps = 3,
            OpenPositionQty = 100,
            ComissionPercent = 0.001m,
        };

    [Test]
    public async Task CanRunBackTestStep()
    {
        var candles = CsvImporter.LoadCandles(DataSources[0].FilePath);
        Assert.That(candles, Is.Not.Null);
        await MarketDataService.LoadData(BackTestParams.Symbol, [.. candles]);

        var candleRange = await MarketDataService.GetCandleRange(BackTestParams.Symbol);
        Assert.That(candleRange, Is.Not.Null);
        Console.WriteLine($"CandleRange={candleRange}");

        var request = new BackTestExecuteStepRequest
        {
            Symbol = BackTestParams.Symbol,
            CurrencyCode = BackTestParams.CurrencyCode,
            PortfolioId = BackTestParams.PortfolioId,
            ComissionPercent = BackTestParams.ComissionPercent,
            OpenPositionQty = BackTestParams.OpenPositionQty,
            Predictor = PredictorType.RandomWalkWithDrift,
            PriceThreshold = 0,
            Time = candleRange.FirstDate,
        };

        var res = await Mediator.Send(request);

        Assert.That(res, Is.Not.Null);

        Console.WriteLine(BackTestResultExtensions.DumpItems(res.Items));
    }
}