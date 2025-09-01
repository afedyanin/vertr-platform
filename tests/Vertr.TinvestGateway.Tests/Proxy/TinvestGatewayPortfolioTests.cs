using Tinkoff.InvestApi;
using Vertr.TinvestGateway.Proxy;

namespace Vertr.TinvestGateway.Tests.Proxy;

[TestFixture(Category = "Gateway", Explicit = true)]
public class TinvestGatewayPortfolioTests
{
    private InvestApiClient _client;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _client = InvestApiClientFactory.Create(Credentials.ApiSettings);
    }

    [Test]
    public async Task CanGetSandboxAccounts()
    {
        var gateway = new TinvestGatewayPortfolio(_client);

        var accounts = await gateway.GetSandboxAccounts() ?? [];

        foreach (var acc in accounts)
        {
            Console.WriteLine(acc);
        }

        Assert.Pass();
    }

    [TestCase("93cda594-5556-44ca-8005-1c893e8d3142")]
    public async Task CanGetSandboxPortfolio(string accountId)
    {
        var gateway = new TinvestGatewayPortfolio(_client);

        var portfolio = await gateway.GetPortfolio(accountId);

        Assert.That(portfolio, Is.Not.Null);

        Console.WriteLine(portfolio);

        foreach (var pos in portfolio.Positions)
        {
            Console.WriteLine(pos);
        }

        Assert.Pass();
    }
}
