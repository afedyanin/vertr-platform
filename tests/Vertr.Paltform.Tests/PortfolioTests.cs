using Vertr.PortfolioManager.Contracts;

namespace Vertr.Paltform.Tests;

[TestFixture(Category = "Application", Explicit = true)]
public class PortfolioTests : ApplicationTestBase
{
    private const string _accountId = "b883ab13-997b-4823-9698-20bac64aeaad";
    private static readonly Guid _bookId = new Guid("D8EBF841-D37B-47C0-AAD3-F778E29B1B85");

    [TestCase(_accountId)]
    public void CanGetInitialPortfolioState(string accountId)
    {
        var portfolio = GetPortfolio(accountId);

        Assert.That(portfolio, Is.Not.Null);
        Assert.That(portfolio.Positions, Is.Not.Null);
        Assert.That(portfolio.Positions, Is.Not.Empty);

        DumpPortfolio(portfolio);
    }

    [TestCase(_accountId)]
    public async Task CanOpenPosition(string accountId)
    {
        _ = await OpenPosition(GetPortfolioIdentity(accountId), 3);
        var portfolio = GetPortfolio(accountId);

        DumpPortfolio(portfolio!);
    }

    [TestCase(_accountId)]
    public async Task CanReversePosition(string accountId)
    {
        _ = await ReversePosition(GetPortfolioIdentity(accountId));
        var portfolio = GetPortfolio(accountId);

        DumpPortfolio(portfolio!);
    }

    private void DumpPortfolio(Portfolio portfolio)
    {
        foreach (var position in portfolio.Positions)
        {
            Console.WriteLine($"Instrument={position.InstrumentId} Balance={position.Balance}");
        }
    }
    private static PortfolioIdentity GetPortfolioIdentity(string accountId) => new PortfolioIdentity(accountId, _bookId);
}
