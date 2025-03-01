using Vertr.Domain.Enums;

namespace Vertr.Adapters.Tinvest.Tests;

[TestFixture(Category = "integration", Explicit = true)]
public class TInvestGatewayAccountRelatedTests : TinvestTestBase
{
    private static readonly Guid _sber_id = new Guid("e6123145-9665-43e0-8413-cd61b8aa9b13");

    [TestCase("fc66cf9b-8fb8-4d9e-ba79-a5e8b87c5aa7")]
    [TestCase("ad518e56-a2d3-46dc-a72b-2cebfac23561")]
    public async Task CanPostMarketOrder(string accountId)
    {
        var requestId = Guid.NewGuid();
        var response = await Gateway.PostOrder(
            accountId,
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

    [TestCase("fc66cf9b-8fb8-4d9e-ba79-a5e8b87c5aa7")]
    [TestCase("ad518e56-a2d3-46dc-a72b-2cebfac23561")]
    public async Task CanGetOrderState(string accountId)
    {
        var requestId = Guid.NewGuid();
        var response = await Gateway.PostOrder(
            accountId,
            _sber_id,
            requestId,
            OrderDirection.Buy,
            OrderType.Market,
            TimeInForceType.Unspecified,
            PriceType.Currency,
            decimal.Zero,
            10);

        var state = await Gateway.GetOrderState(accountId, response.OrderId, PriceType.Unspecified);

        Assert.That(state, Is.Not.Null);

        Console.WriteLine(state);

        // Assert.That(state.OrderStages.Count(), Is.GreaterThan(0));
        foreach (var stage in state.OrderStages)
        {
            Console.WriteLine($"--> {stage}");
        }
    }

    [TestCase("fc66cf9b-8fb8-4d9e-ba79-a5e8b87c5aa7")]
    [TestCase("ad518e56-a2d3-46dc-a72b-2cebfac23561")]
    public async Task CanGetOperations(string accountId)
    {
        var ops = await Gateway.GetOperations(accountId);
        Assert.That(ops, Is.Not.Null);
        Console.WriteLine($"operations count={ops.Count()}");

        /*
        foreach (var op in ops)
        {
            Console.WriteLine("=============");
            Console.WriteLine(op);

            foreach (var trade in op.OperationTrades)
            {
                Console.WriteLine($"--> {trade}");
            }
        }*/
    }

    [TestCase("fc66cf9b-8fb8-4d9e-ba79-a5e8b87c5aa7", "2/28/2025 4:23:03 PM")]
    public async Task CanGetLastOperations(string accountId, string dateFrom)
    {
        var from = DateTime.Parse(dateFrom).ToUniversalTime();
        Console.WriteLine($"DateFrom={from:O}");

        var ops = await Gateway.GetOperations(accountId, from, DateTime.UtcNow);

        Assert.That(ops, Is.Not.Null);
        Console.WriteLine($"operations count={ops.Count()}");

        foreach (var op in ops)
        {
            Console.WriteLine(op);
        }
    }

    [TestCase("fc66cf9b-8fb8-4d9e-ba79-a5e8b87c5aa7")]
    [TestCase("ad518e56-a2d3-46dc-a72b-2cebfac23561")]
    public async Task CanGetPositions(string accountId)
    {
        var positions = await Gateway.GetPositions(accountId);
        Assert.That(positions, Is.Not.Null);

        foreach(var pos in positions)
        {
            Console.WriteLine(pos);
        }
    }

    [TestCase("fc66cf9b-8fb8-4d9e-ba79-a5e8b87c5aa7")]
    [TestCase("ad518e56-a2d3-46dc-a72b-2cebfac23561")]
    public async Task CanGetPortfolio(string accountId)
    {
        var portfolio = await Gateway.GetPortfolio(accountId);
        Assert.That(portfolio, Is.Not.Null);

        Console.WriteLine(portfolio);

        foreach (var pos in portfolio.Positions)
        {
            Console.WriteLine(pos);
        }
    }
}
