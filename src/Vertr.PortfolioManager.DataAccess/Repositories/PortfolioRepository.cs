using Vertr.OrderExecution.Contracts;
using Vertr.PortfolioManager.Application.Abstractions;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.DataAccess.Repositories;

internal class PortfolioRepository : IPortfolioRepository
{
    public Task<PortfolioSnapshot?> GetPortfolio(PortfolioIdentity portfolioIdentity)
    {
        throw new NotImplementedException();
    }

    public Task Remove(PortfolioIdentity portfolioIdentity)
    {
        throw new NotImplementedException();
    }

    public Task SetPortfolio(PortfolioSnapshot portfolio)
    {
        throw new NotImplementedException();
    }
}
