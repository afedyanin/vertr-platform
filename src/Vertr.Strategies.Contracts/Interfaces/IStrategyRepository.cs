namespace Vertr.Strategies.Contracts.Interfaces;
public interface IStrategyRepository
{
    public Task<IStrategy[]> GetActiveStrategies();

    public Task Update(StrategyMetadata strategyMetadata, CancellationToken cancellationToken = default);

    public Task Delete(Guid strategyId, CancellationToken cancellationToken = default);
}
