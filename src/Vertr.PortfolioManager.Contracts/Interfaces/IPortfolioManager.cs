namespace Vertr.PortfolioManager.Contracts.Interfaces;

public interface IPortfolioManager
{
    public Task<string[]> GetActiveAccounts();

    public Task<PortfolioSnapshot?> GetPortfolio(string accountId, Guid? bookId = null);

    public Task DeleteOperationEvents(string accountId, Guid? bookId = null);
}
