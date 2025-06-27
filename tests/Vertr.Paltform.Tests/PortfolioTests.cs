using Vertr.PortfolioManager.Contracts;

namespace Vertr.Paltform.Tests;

[TestFixture(Category = "Application", Explicit = true)]
public class PortfolioTests : ApplicationTestBase
{
    private static readonly Guid _bookId = Guid.NewGuid();
    private const string _accountId = "b883ab13-997b-4823-9698-20bac64aeaad";

    [Test]
    public async Task CanOpenAccount()
    {
        var accountId = await OpenAccount();
        Assert.That(accountId, Is.Not.Null);
        Console.WriteLine($"AccountId={accountId}");
    }

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

    private void DumpPortfolio(PortfolioSnapshot portfolio)
    {
        foreach (var position in portfolio.Positions)
        {
            Console.WriteLine($"Instrument={position.InstrumentIdentity.ClassCode}.{position.InstrumentIdentity.Ticker} Balance={position.Balance}");
        }
    }
    private static PortfolioIdentity GetPortfolioIdentity(string accountId) => new PortfolioIdentity(accountId, _bookId);
}
