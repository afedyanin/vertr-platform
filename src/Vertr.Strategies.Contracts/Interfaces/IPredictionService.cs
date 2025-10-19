using Microsoft.Data.Analysis;

namespace Vertr.Strategies.Contracts.Interfaces;
public interface IPredictionService
{
    public Task<string?> Predict(StrategyType strategyType, string content);

    public Task<DataFrame?> Predict(StrategyType strategyType, DataFrame dataFrame);
}
