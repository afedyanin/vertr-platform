using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.Orders;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.DataAccess.Redis;

internal class OrderStateRepository : RedisRepositoryBase, IOrderStateRepository
{
    private const string StatesKey = "order.states";

    public OrderStateRepository(IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer)
    {
    }

    public async Task Save(OrderState orderState)
    {
        var stateEntry = new HashEntry(GetEntryKey(orderState), orderState.ToJson());
        await GetDatabase().HashSetAsync(StatesKey, [stateEntry]);
    }

    public async Task<IEnumerable<OrderState?>> Find(string pattern)
    {
        var res = new List<OrderState>();

        await foreach (var entry in GetDatabase().HashScanAsync(StatesKey, pattern))
        {
            if (entry.Value.IsNullOrEmpty)
            {
                continue;
            }

            var restored = OrderState.FromJson(entry.Value.ToString());
            if (restored == null)
            {
                continue;
            }

            res.Add(restored);
        }

        return res;
    }

    public Task Clear()
        => GetDatabase().KeyDeleteAsync(StatesKey);

    private static string GetEntryKey(OrderState orderState)
    {
        var emptyId = Guid.Empty.ToString();
        var orderId = string.IsNullOrEmpty(orderState.OrderId) ?
            emptyId : orderState.OrderId;

        var requestId = string.IsNullOrEmpty(orderState.OrderRequestId) ?
            emptyId : orderState.OrderRequestId;

        return $"{orderId}.{requestId}.{orderState.Id}";
    }
}