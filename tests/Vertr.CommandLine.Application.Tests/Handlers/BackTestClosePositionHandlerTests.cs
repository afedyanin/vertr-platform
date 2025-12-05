using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Helpers;
using Vertr.CommandLine.Models.Requests.BackTest;

namespace Vertr.CommandLine.Application.Tests.Handlers;

public class BackTestClosePositionHandlerTests : AppliactionTestBase
{
    [Test]
    public async Task CanClosePositionAfterBackTest()
    {
        var request = new BackTestExecuteStepRequest
        {
            Symbol = "SBER",
            CurrencyCode = "RUB",
            PortfolioId = Guid.NewGuid(),
            Time = new DateTime(2025, 11, 1, 0, 0, 0, DateTimeKind.Utc),
            ComissionPercent = 0.003m,
            OpenPositionQty = 100,
        };

        var res = await Mediator.Send(request);
        Assert.That(res, Is.Not.Null);
        Console.WriteLine($"Before close:\n {BackTestResultExtensions.DumpItems(res.Items)}");

        var closeRequest = new BackTestClosePositionRequest
        {
            CurrencyCode = request.CurrencyCode,
            PortfolioId = request.PortfolioId,
            Symbol = request.Symbol,
            MarketTime = request.Time.AddMinutes(1),
            ComissionPercent = request.ComissionPercent,
        };

        var closeRes = await Mediator.Send(closeRequest);

        Assert.That(closeRes, Is.Not.Null);
        Console.WriteLine($"\nAfter close:\n {BackTestResultExtensions.DumpItems(closeRes.Items)}");

        var positions = closeRes.Items[BackTestContextKeys.Positions] as Position[];
        Assert.That(positions, Is.Not.Null);

        var tradingPosition = positions.FirstOrDefault(p => p.Symbol == request.Symbol);
        Assert.That(tradingPosition, Is.Not.Null);
        Assert.That(tradingPosition.Qty, Is.EqualTo(decimal.Zero));
    }
}