using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.Orders;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.DataAccess.Redis;

internal class OrderTradeRepository : RedisRepositoryBase, IOrderTradeRepository
{
    private const string TradesKey = "order.trades";

    public OrderTradeRepository(IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer)
    {
    }

    public async Task Save(OrderTrades orderTrades)
    {
        var tradesEntry = new HashEntry(GetEntryKey(orderTrades), orderTrades.ToJson());
        await GetDatabase().HashSetAsync(TradesKey, [tradesEntry]);
    }

    public async Task<IEnumerable<OrderTrades?>> Find(string pattern)
    {
        var res = new List<OrderTrades>();

        await foreach (var entry in GetDatabase().HashScanAsync(TradesKey, pattern))
        {
            if (entry.Value.IsNullOrEmpty)
            {
                continue;
            }

            var restored = OrderTrades.FromJson(entry.Value.ToString());
            if (restored == null)
            {
                continue;
            }

            res.Add(restored);
        }

        return res;
    }

    public Task Clear() =>
        GetDatabase().KeyDeleteAsync(TradesKey);

    private static string GetEntryKey(OrderTrades orderTrades)
        => $"{orderTrades.OrderId}.{orderTrades.Id}";
}