using Vertr.OrderExecution.Contracts;

namespace Vertr.PortfolioManager.Contracts.Interfaces;

public interface IPortfolioManager
{
    public Task<string[]> GetActiveAccounts();

    public Task<PortfolioSnapshot?> GetPortfolio(PortfolioIdentity portfolioIdentity);

    public Task Remove(PortfolioIdentity portfolioIdentity);
}
