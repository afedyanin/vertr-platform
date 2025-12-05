using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.Orders;
using Vertr.TinvestGateway.Contracts.Orders.Enums;
using Vertr.TinvestGateway.DataAccess.Redis;

namespace Vertr.TinvestGateway.Tests.RedisTests;

public class OrderRequestRepositoryTests
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
        var repo = new OrderRequestRepository(_connectionMultiplexer);
        await repo.Clear();
    }

    [Test]
    public async Task CanSaveOrderRequest()
    {
        var repo = new OrderRequestRepository(_connectionMultiplexer);

        var portfolioId = Guid.NewGuid();
        var orderRequest = CreateOrderRequest(portfolioId);

        await repo.Save(orderRequest, portfolioId);
        var saved = await repo.Get(orderRequest.RequestId);
        Assert.That(saved, Is.Not.Null);

        Console.WriteLine(saved);
    }

    private static PostOrderRequest CreateOrderRequest(Guid portfolioId)
        => new PostOrderRequest
        {
            InstrumentId = Guid.NewGuid(),
            RequestId = Guid.NewGuid(),
            PortfolioId = portfolioId,
            CreatedAt = DateTime.UtcNow,
            OrderDirection = OrderDirection.Buy,
            OrderType = OrderType.Market,
            Price = decimal.Zero,
            QuantityLots = 10,
            PriceType = PriceType.Unspecified,
            TimeInForceType = TimeInForceType.Unspecified,
        };
}