using Refit;

namespace Vertr.PortfolioManager.Contracts;

public interface IPortfolioClient
{
    [Get("/snapshots/{accountId}")]
    public Task<PortfolioSnapshot?> GetLast(string accountId);

    [Get("/snapshots/history/{accountId}")]
    public Task<PortfolioSnapshot?> GetHistory(string accountId, int maxRecords = 100);

    [Post("/snapshots/{accountId}")]
    public Task<PortfolioSnapshot?> MakeSnapshot(string accountId);
}
