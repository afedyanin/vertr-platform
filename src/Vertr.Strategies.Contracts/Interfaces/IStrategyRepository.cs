namespace Vertr.Strategies.Contracts.Interfaces;

public interface IStrategyRepository
{
    public StrategyMetadata? GetById(Guid id);

    public StrategyMetadata[] GetAll();

    public void Save(StrategyMetadata metadata);

    public void Delete(Guid Id);
}
