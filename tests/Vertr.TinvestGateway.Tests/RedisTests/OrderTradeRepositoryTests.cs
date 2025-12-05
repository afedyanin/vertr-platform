using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.Orders;
using Vertr.TinvestGateway.Contracts.Orders.Enums;
using Vertr.TinvestGateway.Contracts.Portfolios;
using Vertr.TinvestGateway.DataAccess.Redis;

namespace Vertr.TinvestGateway.Tests.RedisTests;

public class OrderTradeRepositoryTests
{
    private IConnectionMultiplexer _connectionMultiplexer;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _connectionMultiplexer = ConnectionMultiplexer.Connect("localhost");
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _connectionMultiplexer.Close();
    }

    [TearDown]
    public async Task TearDown()
    {
        var repo = new OrderTradeRepository(_connectionMultiplexer);
        await repo.Clear();
    }

    [Test]
    public async Task CanSaveOrderTrades()
    {
        var repo = new OrderTradeRepository(_connectionMultiplexer);
        var trades = CreateOrderTrades(Guid.NewGuid());

        await repo.Save(trades);
        var saved = await repo.Find($"*{trades.Id}*");
        Assert.That(saved, Is.Not.Null);

        Console.WriteLine(saved);
    }

    [Test]
    public async Task CanGetOrderTradesByOrderId()
    {
        var repo = new OrderTradeRepository(_connectionMultiplexer);
        var orderId = Guid.NewGuid();
        var ot1 = CreateOrderTrades(orderId);
        var ot2 = CreateOrderTrades(orderId);

        await repo.Save(ot1);
        await repo.Save(ot2);

        var items = await repo.Find($"*{orderId}*");

        Assert.That(items.Count, Is.EqualTo(2));

        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
    }

    private static OrderTrades CreateOrderTrades(Guid orderId, int tradesCount = 2)
        => new OrderTrades
        {
            Id = Guid.NewGuid(),
            OrderId = orderId.ToString(),
            InstrumentId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Direction = OrderDirection.Buy,
            Trades = CreateTrades(tradesCount).ToArray(),
        };

    private static IEnumerable<Trade> CreateTrades(int count)
    {
        var res = new List<Trade>();

        for (var i = 0; i <= count; i++)
        {
            res.Add(CreateTrade());
        }

        return res;
    }

    private static Trade CreateTrade()
        => new Trade
        {
            TradeId = Guid.NewGuid().ToString(),
            ExecutionTime = DateTime.UtcNow,
            Price = new Money(123.45m, "RUB"),
            Quantity = 34,
        };
}