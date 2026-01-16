using StackExchange.Redis;
using Vertr.Common.Contracts;
using Vertr.Common.DataAccess.Repositories;

namespace Vertr.Common.DataAccess.Redis;

internal sealed class OpenInterestRepository : RedisRepositoryBase, IOpenInterestRepository
{
    private const string OpenInterestKey = "market.openInterest";
    private static readonly RedisChannel OpenInterestChannel = new RedisChannel(OpenInterestKey, RedisChannel.PatternMode.Literal);

    public OpenInterestRepository(
        IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer)
    {
    }

    public async Task Save(OpenInterest marketTrade)
    {
        var db = GetDatabase();
        var json = marketTrade.ToJson();
        var obEntry = new HashEntry(marketTrade.InstrumentId.ToString(), json);

        await Task.WhenAll(
            db.HashSetAsync(OpenInterestKey, [obEntry]),
            db.PublishAsync(OpenInterestChannel, json));
    }

    public async Task<IEnumerable<OpenInterest>> GetAll()
    {
        var entries = await GetDatabase().HashGetAllAsync(OpenInterestKey);
        var res = new List<OpenInterest>();

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

            var item = OpenInterest.FromJson(entry.Value.ToString());

            if (item == null)
            {
                continue;
            }

            res.Add(item);
        }

        return res;
    }

    public async Task<OpenInterest?> Get(Guid instrumentId)
    {
        var entry = await GetDatabase().HashGetAsync(OpenInterestKey, instrumentId.ToString());

        if (entry.IsNullOrEmpty)
        {
            return null;
        }

        var restored = OpenInterest.FromJson(entry.ToString());
        return restored;
    }

    public Task Clear()
        => GetDatabase().KeyDeleteAsync(OpenInterestKey);
}
