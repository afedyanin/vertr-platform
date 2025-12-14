using StackExchange.Redis;
using Vertr.Common.Contracts;
using Vertr.Common.Contracts.Converters;
using Vertr.Common.DataAccess.Repositories;

namespace Vertr.Common.DataAccess.Redis;

internal sealed class CandlestickRepository : RedisRepositoryBase, ICandlestickRepository
{
    private const string PrefixKey = "market.candles";

    public CandlestickRepository(IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer)
    {
    }

    public async Task<long> Save(Guid instrumentId, Candlestick[] candles, int maxCount = 0, bool publish = true)
    {
        var trimmedCandles = maxCount > 0 ? candles.OrderBy(c => c.Time).TakeLast(maxCount) : candles;

        var db = GetDatabase();
        var key = GetKey(instrumentId);

        if (maxCount > 0)
        {
            var currentItemsCount = await db.SortedSetLengthAsync(key);
            var toRemove = (currentItemsCount + trimmedCandles.Count()) - maxCount;

            if (toRemove > 0)
            {
                var removed = await db.SortedSetRemoveRangeByRankAsync(key, 0, toRemove - 1);
            }
        }

        var entries = trimmedCandles.Select(c => new SortedSetEntry(c.ToJson(), c.Time)).ToArray();
        var added = await db.SortedSetAddAsync(GetKey(instrumentId), entries);

        if (publish)
        {
            // publish last candle
            var last = trimmedCandles.OrderBy(c => c.Time).Last();
            var candle = last.ToCandle(instrumentId);
            var channel = new RedisChannel(key.ToString(), RedisChannel.PatternMode.Pattern);
            await db.PublishAsync(channel, candle.ToJson());
        }

        return added;
    }

    public async Task<IEnumerable<Candlestick?>> GetLast(Guid instrumentId, long maxItems = -1)
    {
        var items = await GetDatabase().SortedSetRangeByRankAsync(GetKey(instrumentId), 0, maxItems, StackExchange.Redis.Order.Descending);
        return items.Select(c => Candlestick.FromJson(c.ToString()));
    }

    public Task<bool> Clear(Guid instrumentId)
        => GetDatabase().KeyDeleteAsync(GetKey(instrumentId));

    private static RedisKey GetKey(Guid instrumentId) => new($"{PrefixKey}.{instrumentId}");
}