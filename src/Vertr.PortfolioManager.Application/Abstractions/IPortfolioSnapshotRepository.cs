using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.Application.Abstractions;
public interface IPortfolioSnapshotRepository
{
    public Task<PortfolioSnapshot?> GetLast(string accountId, Guid? bookId = null);

    public Task<PortfolioSnapshot[]> GetHistory(string accountId, Guid? bookId = null, int maxRecords = 100);

    public Task<bool> Save(PortfolioSnapshot portfolio);

    public Task<bool> Delete(Guid id);

    public Task<int> DeleteByAccountId(string accountId);

    public Task<int> DeleteByBookId(Guid bookId);
}
