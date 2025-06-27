namespace Vertr.PortfolioManager.Contracts.Interfaces;
public interface IPortfolioRepository
{
    public Task<string[]> GetActiveAccounts();

    public Task<Portfolio?> GetPortfolio(PortfolioIdentity portfolioIdentity);

    public Task SetPortfolio(Portfolio portfolio);

    public Task Remove(PortfolioIdentity portfolioIdentity);
}
