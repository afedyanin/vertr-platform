using Microsoft.Data.Analysis;
using Vertr.MarketData.Contracts;

namespace Vertr.Strategies.Contracts.Interfaces;
public interface IPredictionService
{
    public Task<string?> Predict(StrategyType strategyType, string content);

    public Task<DataFrame?> Predict(StrategyType strategyType, DataFrame dataFrame);

    public Task<DataFrame?> Predict(StrategyType strategyType, IEnumerable<Candle> candles);
}
