namespace Vertr.PortfolioManager.Contracts.Interfaces;

public interface IPortfolioManager
{
    public Task<string[]> GetActiveAccounts();

    public Task<Portfolio?> GetPortfolio(PortfolioIdentity portfolioIdentity);

    public Task Remove(PortfolioIdentity portfolioIdentity, bool deleteOperations = false);
}
