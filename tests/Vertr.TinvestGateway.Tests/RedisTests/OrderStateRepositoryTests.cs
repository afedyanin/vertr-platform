using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.Orders;
using Vertr.TinvestGateway.DataAccess.Redis;

namespace Vertr.TinvestGateway.Tests.RedisTests;

public class OrderStateRepositoryTests
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
        var repo = new OrderStateRepository(_connectionMultiplexer);
        await repo.Clear();
    }

    [Test]
    public async Task CanSaveOrderState()
    {
        var repo = new OrderStateRepository(_connectionMultiplexer);
        var orderId = Guid.NewGuid();
        var s1 = CreateOrderState(orderId.ToString());

        await repo.Save(s1);
        var found = await repo.Find($"*{s1.Id}*");
        Assert.That(found, Is.Not.Null);

        foreach (var item in found)
        {
            Console.WriteLine(item);
        }
    }

    [Test]
    public async Task CanGetOrderStatesByOrderId()
    {
        var repo = new OrderStateRepository(_connectionMultiplexer);
        var orderId = Guid.NewGuid();
        var os1 = CreateOrderState(orderId.ToString());
        var os2 = CreateOrderState(orderId.ToString());

        await repo.Save(os1);
        await repo.Save(os2);

        var found = await repo.Find($"*{orderId}*");

        Assert.That(found.Count, Is.EqualTo(2));

        foreach (var item in found)
        {
            Console.WriteLine(item);
        }
    }

    [Test]
    public async Task CanGetOrderStatesByOrderIdAndRequestId()
    {
        var repo = new OrderStateRepository(_connectionMultiplexer);
        var orderId = Guid.NewGuid();
        var requestId = Guid.NewGuid();
        var os1 = CreateOrderState(orderId.ToString());
        var os2 = CreateOrderState(orderId.ToString(), requestId.ToString());
        var os3 = CreateOrderState("", requestId.ToString());

        await repo.Save(os1);
        await repo.Save(os2);
        await repo.Save(os3);

        var foundbyOrderId = await repo.Find($"*{orderId}*");
        Assert.That(foundbyOrderId.Count, Is.EqualTo(2));
        foreach (var item in foundbyOrderId)
        {
            Console.WriteLine(item);
        }

        var foundbyRequestId = await repo.Find($"*{requestId}*");
        Assert.That(foundbyRequestId.Count, Is.EqualTo(2));

        foreach (var item in foundbyRequestId)
        {
            Console.WriteLine(item);
        }
    }

    private static OrderState CreateOrderState(string orderId = "", string requestId = "")
    {
        var state = new OrderState
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            InstrumentId = Guid.NewGuid(),
            OrderRequestId = requestId,
            CreatedAt = DateTime.UtcNow,
        };

        return state;
    }
}