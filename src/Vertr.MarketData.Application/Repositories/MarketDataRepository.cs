using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.MarketData.Application.Repositories;

// Not thread safe 
internal class MarketDataRepository : IMarketDataRepository
{
    private readonly Dictionary<string, CandleRepository> _repoDict = [];
    private readonly int _maxCandlesCapacity;

    public MarketDataRepository(int maxCandlesCapacity = 1000)
    {
        _maxCandlesCapacity = maxCandlesCapacity;
    }

    public void Add(Symbol symbol, CandleInterval interval, Candle candle)
    {
        var repoKey = GetKey(symbol, interval);

        if (!_repoDict.TryGetValue(repoKey, out var repo))
        {
            repo = new CandleRepository(_maxCandlesCapacity);
            _repoDict[repoKey] = repo;
        }

        repo.Add(candle);
    }

    public void AddRange(Symbol symbol, CandleInterval interval, Candle[] candles)
    {
        var repoKey = GetKey(symbol, interval);

        if (!_repoDict.TryGetValue(repoKey, out var repo))
        {
            repo = new CandleRepository(_maxCandlesCapacity);
            _repoDict[repoKey] = repo;
        }

        repo.AddRange(candles);
    }

    public Candle[] GetAll(Symbol symbol, CandleInterval interval, int maxCount = 0)
    {
        var repoKey = GetKey(symbol, interval);

        if (!_repoDict.TryGetValue(repoKey, out var repo))
        {
            return [];
        }

        return repo.GetAll(maxCount);
    }

    public Candle? GetLast(Symbol symbol, CandleInterval interval)
    {
        var repoKey = GetKey(symbol, interval);

        if (!_repoDict.TryGetValue(repoKey, out var repo))
        {
            return null;
        }

        return repo.GetLast();
    }

    private static string GetKey(Symbol symbol, CandleInterval interval)
        => $"{symbol.ClassCode}.{symbol.Ticker}.{interval}";
}
