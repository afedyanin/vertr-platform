using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.PortfolioManager.DataAccess.Repositories;

internal class PortfolioRepository : IPortfolioRepository
{
    public Task<Portfolio?> GetPortfolio(PortfolioIdentity portfolioIdentity)
    {
        throw new NotImplementedException();
    }

    public Task Remove(PortfolioIdentity portfolioIdentity)
    {
        throw new NotImplementedException();
    }

    public Task SetPortfolio(Portfolio portfolio)
    {
        throw new NotImplementedException();
    }
}
