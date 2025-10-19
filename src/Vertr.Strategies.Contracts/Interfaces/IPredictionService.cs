namespace Vertr.Strategies.Contracts.Interfaces;
public interface IPredictionService
{
    public Task<string> Predict(StrategyType strategyType, string content);
}
