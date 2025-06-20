namespace Vertr.PortfolioManager.Contracts.Interfaces;

public interface IPortfolioManager
{
    public Task<string[]> GetActiveAccounts();

    public Task<PortfolioSnapshot?> GetLastPortfolio(string accountId, Guid? bookId = null);

    public Task<PortfolioSnapshot[]> GetPortfolioHistory(string accountId, Guid? bookId = null, int maxRecords = 100);

    public Task<PortfolioSnapshot?> MakeSnapshot(string accountId, Guid? bookId = null);
}
