namespace Vertr.Strategies.Contracts.Interfaces;
public interface IStrategyRepository
{
    public Task<IStrategy[]> GetActiveStrategies();

    public Task Update(StrategyMetadata strategyMetadata);
}
