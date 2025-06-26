using System.Text.Json;
using StackExchange.Redis;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.MarketData.DataAccess.Repositories;

public class MarketInstrumentRepository : IMarketInstrumentRepository
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

    public Task<Instrument?> Get(InstrumentIdentity instrumentIdentity)
    {
        throw new NotImplementedException();
    }

    public Task<Instrument[]> GetAll()
    {
        throw new NotImplementedException();
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
        // TODO: Use scan
        var idKeys = await _database.HashKeysAsync(_instrumentsByIdKey);

        foreach (var idKey in idKeys)
        {
            await _database.HashDeleteAsync(_instrumentsByIdKey, idKey);
        }

        // TODO: Use scan
        var tickerKeys = await _database.HashKeysAsync(_instrumentsByTickerKey);

        foreach (var tickerKey in tickerKeys)
        {
            await _database.HashDeleteAsync(_instrumentsByIdKey, tickerKey);
        }
    }

    private static string GetTickerKey(InstrumentIdentity instrumentIdentity)
        => $"{instrumentIdentity.ClassCode}:{instrumentIdentity.Ticker}";

    private static string GetIdKey(InstrumentIdentity instrumentIdentity)
        => $"{instrumentIdentity.Id}";
}
