using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.MarketData;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.DataAccess.Redis;

internal class InstrumentRepository : RedisRepositoryBase, IInstrumentRepository
{
    private const string InstrumentsKey = "market.instruments";

    public InstrumentRepository(IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer)
    {
    }

    public async Task Save(Instrument instrument)
    {
        var instrumentEntry = new HashEntry(instrument.Id.ToString(), instrument.ToJson());
        await GetDatabase().HashSetAsync(InstrumentsKey, [instrumentEntry]);
    }

    public async Task<IEnumerable<Instrument>> GetAll()
    {
        var entries = await GetDatabase().HashGetAllAsync(InstrumentsKey);
        var res = new List<Instrument>();

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

            var item = Instrument.FromJson(entry.Value.ToString());

            if (item == null)
            {
                continue;
            }

            res.Add(item);
        }

        return res;
    }

    public async Task<Instrument?> Get(Guid id)
    {
        var entry = await GetDatabase().HashGetAsync(InstrumentsKey, id.ToString());

        if (entry.IsNullOrEmpty)
        {
            return null;
        }

        var restored = Instrument.FromJson(entry.ToString());
        return restored;
    }

    public Task Clear()
        => GetDatabase().KeyDeleteAsync(InstrumentsKey);
}