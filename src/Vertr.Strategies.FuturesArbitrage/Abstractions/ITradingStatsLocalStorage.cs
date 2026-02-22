using Vertr.Strategies.FuturesArbitrage.Models;

namespace Vertr.Strategies.FuturesArbitrage.Abstractions;

public interface ITradingStatsLocalStorage
{
    public IEnumerable<TradingStatsInfo> GetAll();

    public void Save(IEnumerable<TradingStatsInfo> tradingStatsInfos);

    public void Clear();
}
