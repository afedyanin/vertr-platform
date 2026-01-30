using StackExchange.Redis;
using Vertr.Common.Contracts;
using Vertr.Common.DataAccess.Repositories;

namespace Vertr.TinvestGateway.DataAccess.Redis;


internal sealed class MarketTradeRepository : RedisRepositoryBase, IMarketTradeRepository
{
    private const string TradesKey = "market.trades";
    private static readonly RedisChannel TradesChannel = new RedisChannel(TradesKey, RedisChannel.PatternMode.Literal);

    public MarketTradeRepository(
        IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer)
    {
    }

    public async Task Save(MarketTrade marketTrade)
    {
        var db = GetDatabase();
        var json = marketTrade.ToJson();
        var obEntry = new HashEntry(marketTrade.InstrumentId.ToString(), json);

        await Task.WhenAll(
            db.HashSetAsync(TradesKey, [obEntry]),
            db.PublishAsync(TradesChannel, json));
    }

    public async Task<IEnumerable<MarketTrade>> GetAll()
    {
        var entries = await GetDatabase().HashGetAllAsync(TradesKey);
        var res = new List<MarketTrade>();

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

            var item = MarketTrade.FromJson(entry.Value.ToString());

            if (item == null)
            {
                continue;
            }

            res.Add(item);
        }

        return res;
    }

    public async Task<MarketTrade?> Get(Guid instrumentId)
    {
        var entry = await GetDatabase().HashGetAsync(TradesKey, instrumentId.ToString());

        if (entry.IsNullOrEmpty)
        {
            return null;
        }

        var restored = MarketTrade.FromJson(entry.ToString());
        return restored;
    }

    public Task Clear()
        => GetDatabase().KeyDeleteAsync(TradesKey);
}
