using Refit;
using Vertr.TinvestGateway.Contracts;

namespace Vertr.TinvestGateway.Tests;

public class GatewayClientTests
{
    [Test]
    public async Task CanGetAccounts()
    {
        var gateway = RestService.For<ITinvestGateway>("http://localhost:5125");
        var accounts = await gateway.GetAccounts();

        Assert.That(accounts, Is.Not.Empty);

        foreach (var acc in accounts)
        {
            Console.WriteLine(acc);
        }
    }
}
