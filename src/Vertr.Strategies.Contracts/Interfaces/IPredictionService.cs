using Vertr.MarketData.Contracts;

namespace Vertr.Strategies.Contracts.Interfaces;
public interface IPredictionService
{
    public Task<PredictionResult> Predict(StrategyType strategyType, string content);

    public Task<PredictionResult> Predict(StrategyType strategyType, IEnumerable<Candle> candles);
}
