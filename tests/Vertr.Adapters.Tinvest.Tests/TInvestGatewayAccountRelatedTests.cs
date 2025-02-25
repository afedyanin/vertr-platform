using Vertr.Domain;
using Vertr.Domain.Enums;

namespace Vertr.Adapters.Tinvest.Tests;

[TestFixture(Category = "integration", Explicit = true)]
public class TInvestGatewayAccountRelatedTests : TinvestTestBase
{
    private const string _sber_id = "e6123145-9665-43e0-8413-cd61b8aa9b13";

    private const string _accountId = "ad518e56-a2d3-46dc-a72b-2cebfac23561";

    [Test]
    public async Task CanPrepareSandboxAccount()
    {
        var accountId = await PrepareSandboxAccount();
        Console.WriteLine($"Account created. AccountId={accountId}");
    }

    [Test]
    public async Task CanPostMarketOrder()
    {
        var requestId = Guid.NewGuid();
        var response = await Gateway.PostOrder(
            _accountId,
            _sber_id,
            requestId,
            OrderDirection.Buy,
            OrderType.Market,
            TimeInForceType.Unspecified,
            PriceType.Currency,
            decimal.Zero,
            10);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.OrderRequestId, Is.EqualTo(requestId.ToString()));

        Console.WriteLine(response);
    }

    [Test]
    public async Task CanGetOrderState()
    {
        var requestId = Guid.NewGuid();
        var response = await Gateway.PostOrder(
            _accountId,
            _sber_id,
            requestId,
            OrderDirection.Buy,
            OrderType.Market,
            TimeInForceType.Unspecified,
            PriceType.Currency,
            decimal.Zero,
            10);

        var status = await Gateway.GetOrderState(_accountId, response.OrderId, PriceType.Unspecified);

        Assert.That(status, Is.Not.Null);

        Console.WriteLine(status);
    }

    [Test]
    public async Task CanGetOperations()
    {
        var ops = await Gateway.GetOperations(_accountId);
        Assert.That(ops, Is.Not.Null);

        foreach(var op in ops)
        {
            Console.WriteLine(op);
        }
    }

    [Test]
    public async Task CanGetPositions()
    {
        var positions = await Gateway.GetPositions(_accountId);
        Assert.That(positions, Is.Not.Null);

        foreach(var pos in positions)
        {
            Console.WriteLine(pos);
        }
    }

    [Test]
    public async Task CanGetPortfolio()
    {
        var portfolio = await Gateway.GetPortfolio(_accountId);
        Assert.That(portfolio, Is.Not.Null);

        Console.WriteLine(portfolio);

        foreach (var pos in portfolio.Positions)
        {
            Console.WriteLine(pos);
        }
    }

    private async Task<string> PrepareSandboxAccount()
    {
        var accountId = await Gateway.CreateSandboxAccount("Test account");
        var amount = new Money
        {
            Currency = "rub",
            Value = 100_000m,
        };

        var balance = await Gateway.SandboxPayIn(accountId, amount);

        return accountId;
    }
}
