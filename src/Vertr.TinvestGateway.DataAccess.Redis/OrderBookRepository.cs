using StackExchange.Redis;
using Vertr.Common.Contracts;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.DataAccess.Redis;

internal class OrderBookRepository : RedisRepositoryBase, IOrderBookRepository
{
    private const string OrderBooksKey = "market.orderBooks";
    private static readonly RedisChannel OrderBooksChannel = new RedisChannel(OrderBooksKey, RedisChannel.PatternMode.Literal);

    public OrderBookRepository(
        IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer)
    {
    }
    public async Task Save(OrderBook orderBook)
    {
        var db = GetDatabase();
        var json = orderBook.ToJson();
        var obEntry = new HashEntry(orderBook.InstrumentId.ToString(), json);

        await Task.WhenAll(
            db.HashSetAsync(OrderBooksKey, [obEntry]),
            db.PublishAsync(OrderBooksChannel, json));
    }

    public async Task<IEnumerable<OrderBook>> GetAll()
    {
        var entries = await GetDatabase().HashGetAllAsync(OrderBooksKey);
        var res = new List<OrderBook>();

        if (entries == null)
        {
            return [];
        }

        foreach (var entry in entries)
        {
            if (entry.Value.IsNullOrEmpty)
            {
                continue;
            }

            var item = OrderBook.FromJson(entry.Value.ToString());

            if (item == null)
            {
                continue;
            }

            res.Add(item);
        }

        return res;
    }

    public async Task<OrderBook?> Get(Guid instrumentId)
    {
        var entry = await GetDatabase().HashGetAsync(OrderBooksKey, instrumentId.ToString());

        if (entry.IsNullOrEmpty)
        {
            return null;
        }

        var restored = OrderBook.FromJson(entry.ToString());
        return restored;
    }
    public Task Clear()
        => GetDatabase().KeyDeleteAsync(OrderBooksKey);
}
