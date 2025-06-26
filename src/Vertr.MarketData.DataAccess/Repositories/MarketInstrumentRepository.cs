using System.Text.Json;
using StackExchange.Redis;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.MarketData.DataAccess.Converters;

namespace Vertr.MarketData.DataAccess.Repositories;

public class MarketInstrumentRepository : IStaticMarketDataRepository
{
    private const string _instrumentsByIdKey = "md:instruments:id";
    private const string _instrumentsByTickerKey = "md:instruments:ticker";

    private readonly IDatabaseAsync _database;
    private readonly IConnectionMultiplexer _redis;

    public MarketInstrumentRepository(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _database = _redis.GetDatabase();
    }

    public async Task<Instrument?> Get(InstrumentIdentity instrumentIdentity)
    {
        // TODO: use in memory cache?
        if (instrumentIdentity.Id.HasValue)
        {
            var idKey = GetIdKey(instrumentIdentity);
            var resByKey = await _database.HashGetAsync(_instrumentsByIdKey, idKey);
            if (resByKey.HasValue)
            {
                return resByKey.Convert();
            }
        }

        var tickerKey = GetTickerKey(instrumentIdentity);
        var resByTicker = await _database.HashGetAsync(_instrumentsByTickerKey, tickerKey);

        if (resByTicker.HasValue)
        {
            return resByTicker.Convert();
        }

        return null;
    }

    public async Task<Instrument[]?> GetAll()
    {
        var res = await _database.HashGetAllAsync(_instrumentsByIdKey);
        return res.Convert();
    }

    public async Task Save(Instrument[] instruments)
    {
        var hashId = new List<HashEntry>();
        var hashTicker = new List<HashEntry>();

        foreach (var item in instruments)
        {
            var value = new RedisValue(JsonSerializer.Serialize(item));
            var idEntry = new HashEntry(GetIdKey(item.InstrumentIdentity), value);
            var tickerEntry = new HashEntry(GetTickerKey(item.InstrumentIdentity), value);

            hashId.Add(idEntry);
            hashTicker.Add(tickerEntry);
        }

        await _database.HashSetAsync(_instrumentsByIdKey, [.. hashId]);
        await _database.HashSetAsync(_instrumentsByTickerKey, [.. hashTicker]);
    }

    public async Task Clear()
    {
        await _database.KeyDeleteAsync(_instrumentsByIdKey);
        await _database.KeyDeleteAsync(_instrumentsByTickerKey);
    }

    private static string GetTickerKey(InstrumentIdentity instrumentIdentity)
        => $"{instrumentIdentity.ClassCode}:{instrumentIdentity.Ticker}";

    private static string GetIdKey(InstrumentIdentity instrumentIdentity)
        => $"{instrumentIdentity.Id}";

    public Task<Instrument?> GetInstrument(InstrumentIdentity instrumentIdentity)
    {
        throw new NotImplementedException();
    }

    public Task<CandleInterval?> GetInterval(InstrumentIdentity instrumentIdentity)
    {
        throw new NotImplementedException();
    }

    public Task<Instrument[]?> GetInstruments()
    {
        throw new NotImplementedException();
    }

    public Task<CandleSubscription[]?> GetSubscriptions()
    {
        throw new NotImplementedException();
    }
}
