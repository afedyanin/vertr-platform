using Refit;
using Vertr.OrderExecution.Contracts.Interfaces;

namespace Vertr.OrderExecution.Tests;

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
