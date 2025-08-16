namespace Vertr.Strategies.Contracts.Interfaces;
public interface IStrategyFactory
{
    public IStrategy Create(
        StrategyMetadata strategyMetadata,
        IServiceProvider serviceProvider);
}
