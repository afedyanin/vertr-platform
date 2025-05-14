using Vertr.PortfolioManager.Application.Entities;

namespace Vertr.PortfolioManager.Application.Abstractions;
public interface IPortfolioSnapshotRepository
{
    public Task<PortfolioSnapshot?> GetLast(string accountId);

    public Task<PortfolioSnapshot[]> GetHistory(string accountId, int maxRecords = 100);

    public Task<bool> Save(PortfolioSnapshot portfolio);

    public Task<bool> Delete(Guid snapshotPortfolioId);

    public Task<bool> DeleteAll(string accountId);
}
