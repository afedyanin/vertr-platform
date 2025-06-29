using Vertr.PortfolioManager.Contracts;

namespace Vertr.Platform.Tests;

[TestFixture(Category = "Application", Explicit = true)]
public class PortfolioTests : ApplicationTestBase
{
    private static readonly string _accountId = "dd83bf2e-dac1-4638-a8b3-5d01c32c49b5";
    private static readonly Guid _subAccountId = new Guid("dd83bf2e-dac2-4638-a8b3-5d01c32c49b5");
    private static readonly PortfolioIdentity _identity = new PortfolioIdentity(_accountId, _subAccountId);

    [Test]
    public async Task CanGetInitialPortfolioState()
    {
        var portfolio = await GetPortfolio(_identity);

        Assert.That(portfolio, Is.Not.Null);
        Assert.That(portfolio.Positions, Is.Not.Null);
        Assert.That(portfolio.Positions, Is.Not.Empty);

        DumpPortfolio(portfolio);
    }

    [Test]
    public async Task CanOpenPosition()
    {
        _ = await OpenPosition(_identity, 3);
        var portfolio = await GetPortfolio(_identity);

        DumpPortfolio(portfolio!);
    }

    [Test]
    public async Task CanReversePosition()
    {
        _ = await ReversePosition(_identity);
        var portfolio = await GetPortfolio(_identity);

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
