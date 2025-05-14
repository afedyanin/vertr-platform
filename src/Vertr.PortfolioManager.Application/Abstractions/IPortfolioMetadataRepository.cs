using Vertr.PortfolioManager.Application.Entities;

namespace Vertr.PortfolioManager.Application.Abstractions;

public interface IPortfolioMetadataRepository
{
    public Task<PortfolioMetadata?> GetById(string accountId, Guid? portfolioId);

    public Task<PortfolioMetadata[]> GetAll();

    public Task<bool> Save(PortfolioMetadata portfolioMetadata);

    public Task<bool> Delete(string accountId, Guid? portfolioId);
}
