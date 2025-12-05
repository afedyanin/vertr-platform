using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.Orders;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.DataAccess.Redis;

internal class OrderRequestRepository : RedisRepositoryBase, IOrderRequestRepository
{
    private const string RequestsKey = "order.requests";

    public OrderRequestRepository(IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer)
    {
    }

    public async Task Save(PostOrderRequest orderRequest, Guid portfolioId)
    {
        var requestEntry = new HashEntry(orderRequest.RequestId.ToString(), orderRequest.ToJson());
        await GetDatabase().HashSetAsync(RequestsKey, [requestEntry]);
    }

    public async Task<PostOrderRequest?> Get(Guid id)
    {
        var entry = await GetDatabase().HashGetAsync(RequestsKey, id.ToString());

        if (entry.IsNullOrEmpty)
        {
            return null;
        }

        var restored = PostOrderRequest.FromJson(entry.ToString());
        return restored;
    }

    public async Task<IEnumerable<PostOrderRequest?>> Find(string pattern)
    {
        var res = new List<PostOrderRequest>();

        await foreach (var entry in GetDatabase().HashScanAsync(RequestsKey, pattern))
        {
            if (entry.Value.IsNullOrEmpty)
            {
                continue;
            }

            var restored = PostOrderRequest.FromJson(entry.Value.ToString());
            if (restored == null)
            {
                continue;
            }

            res.Add(restored);
        }

        return res;
    }

    public Task Clear() =>
        GetDatabase().KeyDeleteAsync(RequestsKey);
}