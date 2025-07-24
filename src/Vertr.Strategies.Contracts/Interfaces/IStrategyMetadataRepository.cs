namespace Vertr.Strategies.Contracts.Interfaces;

public interface IStrategyMetadataRepository
{
    public Task<StrategyMetadata[]> GetAll();

    public Task<StrategyMetadata?> GetById(Guid id);

    public Task<bool> Save(StrategyMetadata metadata);

    public Task<int> Delete(Guid Id);
}
