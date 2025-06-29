using Vertr.PortfolioManager.Contracts;

namespace Vertr.Platform.Tests;

[TestFixture(Category = "Application", Explicit = true)]
public class PortfolioTests : ApplicationTestBase
{
    private const string _accountId = "b883ab13-997b-4823-9698-20bac64aeaad";
    private static readonly Guid _subAccountId = new Guid("D8EBF841-D37B-47C0-AAD3-F778E29B1B85");

    [TestCase(_accountId)]
    public async Task CanGetInitialPortfolioState(string accountId)
    {
        var portfolio = await GetPortfolio(accountId);

        Assert.That(portfolio, Is.Not.Null);
        Assert.That(portfolio.Positions, Is.Not.Null);
        Assert.That(portfolio.Positions, Is.Not.Empty);

        DumpPortfolio(portfolio);
    }

    [TestCase(_accountId)]
    public async Task CanOpenPosition(string accountId)
    {
        _ = await OpenPosition(GetPortfolioIdentity(accountId), 3);
        var portfolio = await GetPortfolio(accountId);

        DumpPortfolio(portfolio!);
    }

    [TestCase(_accountId)]
    public async Task CanReversePosition(string accountId)
    {
        _ = await ReversePosition(GetPortfolioIdentity(accountId));
        var portfolio = await GetPortfolio(accountId);

        DumpPortfolio(portfolio!);
    }

    /*
    [TestCase("0e284896-ba30-440f-9626-18ab2e2cc2f0")]
    public async Task CanValidateTradeOperations(string accountId)
    {
    }
    */

    private void DumpPortfolio(Portfolio portfolio)
    {
        foreach (var position in portfolio.Positions)
        {
            Console.WriteLine($"Instrument={position.InstrumentId} Balance={position.Balance}");
        }
    }
    private static PortfolioIdentity GetPortfolioIdentity(string accountId) => new PortfolioIdentity(accountId, _subAccountId);
}
