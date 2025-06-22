using Vertr.PortfolioManager.Contracts;

namespace Vertr.Paltform.Tests;

[TestFixture(Category = "Application", Explicit = true)]
public class PortfolioTests : ApplicationTestBase
{
    [Test]
    public async Task CanGetInitialPortfolioState()
    {
        var portfolio = await GetPortfolio();

        Assert.That(portfolio, Is.Not.Null);
        Assert.That(portfolio.Positions, Is.Not.Null);
        Assert.That(portfolio.Positions, Is.Not.Empty);

        DumpPortfolio(portfolio);
    }

    [Test]
    public async Task CanOpenPosition()
    {
        _ = await OpenPosition(3);
        var portfolio = await GetPortfolio();

        DumpPortfolio(portfolio!);
    }

    private void DumpPortfolio(PortfolioSnapshot portfolio)
    {
        foreach (var position in portfolio.Positions)
        {
            Console.WriteLine($"InstrumentId={position.InstrumentId} Balance={position.Balance}");
        }
    }
}
