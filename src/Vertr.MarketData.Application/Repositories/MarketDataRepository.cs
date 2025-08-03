using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces.old;

namespace Vertr.MarketData.Application.Repositories;

// Not thread safe 
internal class MarketDataRepository : IMarketDataRepository
{
    private readonly Dictionary<Guid, CandleRepository> _repoDict = [];
    private readonly int _maxCandlesCapacity;

    public MarketDataRepository(int maxCandlesCapacity = 1000)
    {
        _maxCandlesCapacity = maxCandlesCapacity;
    }

    public void Add(Candle candle)
    {

        if (!_repoDict.TryGetValue(candle.InstrumentId, out var repo))
        {
            repo = new CandleRepository(_maxCandlesCapacity);
            _repoDict[candle.InstrumentId] = repo;
        }

        repo.Add(candle);
    }

    public void AddRange(Candle[] candles)
    {
        var gropued = candles.GroupBy(c => c.InstrumentId);

        foreach (var group in gropued)
        {
            if (!_repoDict.TryGetValue(group.Key, out var repo))
            {
                repo = new CandleRepository(_maxCandlesCapacity);
                _repoDict[group.Key] = repo;
            }

            repo.AddRange([.. group.Select(s => s)]);
        }
    }

    public Candle[] GetAll(Guid instrumentId, int maxCount = 0)
    {
        if (!_repoDict.TryGetValue(instrumentId, out var repo))
        {
            return [];
        }

        return repo.GetAll(maxCount);
    }

    public Candle? GetLast(Guid instrumentId)
    {
        if (!_repoDict.TryGetValue(instrumentId, out var repo))
        {
            return null;
        }

        return repo.GetLast();
    }
}
