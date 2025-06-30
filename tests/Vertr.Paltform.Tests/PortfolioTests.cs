using Vertr.PortfolioManager.Contracts;

namespace Vertr.Platform.Tests;

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

    [Test]
    public async Task CanClosePosition()
    {
        _ = await ClosePosition();
        var portfolio = await GetPortfolio();

        DumpPortfolio(portfolio!);
    }

    [Test]
    public async Task CanReversePosition()
    {
        _ = await ReversePosition();
        var portfolio = await GetPortfolio();

        DumpPortfolio(portfolio!);
    }

    [Test]
    public async Task CanDepositAmount()
    {
        await PayIn(100000);
        var portfolio = await GetPortfolio();

        DumpPortfolio(portfolio!);
    }

    private void DumpPortfolio(Portfolio portfolio)
    {
        foreach (var position in portfolio.Positions)
        {
            Console.WriteLine($"Instrument={position.InstrumentId} Balance={position.Balance}");
        }
    }
}
