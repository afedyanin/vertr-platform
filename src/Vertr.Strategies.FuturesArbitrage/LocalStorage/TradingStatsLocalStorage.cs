using Vertr.Strategies.FuturesArbitrage.Abstractions;
using Vertr.Strategies.FuturesArbitrage.Models;

internal sealed class TradingStatsLocalStorage : ITradingStatsLocalStorage
{
    private readonly List<TradingStatsInfo> _tradingStats = [];

    public IEnumerable<TradingStatsInfo> GetAll()
        => _tradingStats;

    public void Save(IEnumerable<TradingStatsInfo> tradingStatsInfos)
    {
        _tradingStats.AddRange(tradingStatsInfos);
    }

    public void Clear()
    {
        _tradingStats.Clear();
    }
}
