using Refit;

namespace Vertr.PortfolioManager.Contracts;

public interface IPortfolioClient
{
    [Get("/snapshots/{accountId}")]
    public Task<PortfolioSnapshot?> GetLast(string accountId, Guid? bookId = null);

    [Get("/snapshots/history/{accountId}")]
    public Task<PortfolioSnapshot?> GetHistory(string accountId, Guid? bookId = null, int maxRecords = 100);

    [Post("/snapshots/{accountId}")]
    public Task<PortfolioSnapshot?> MakeSnapshot(string accountId, Guid? bookId = null);
}
