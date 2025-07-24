namespace Vertr.Strategies.Contracts.Interfaces;
public interface IStrategyHostingService
{
    public Task<IStrategy[]> GetActiveStrategies();
}
