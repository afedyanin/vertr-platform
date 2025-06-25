using Vertr.OrderExecution.Contracts;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.Application.Abstractions;
public interface IPortfolioRepository
{
    public Task<PortfolioSnapshot?> GetPortfolio(PortfolioIdentity portfolioIdentity);

    public Task SetPortfolio(PortfolioSnapshot portfolio);

    public Task Remove(PortfolioIdentity portfolioIdentity);
}
