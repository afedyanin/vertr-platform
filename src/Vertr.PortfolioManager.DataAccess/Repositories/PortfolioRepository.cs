using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.PortfolioManager.DataAccess.Repositories;

internal class PortfolioRepository : IPortfolioRepository
{
    // TODO: Move it into DB
    private static readonly string[] _accounts =
        [
            "b883ab13-997b-4823-9698-20bac64aeaad"
        ];

    public Task<string[]> GetActiveAccounts()
        => Task.FromResult(_accounts);

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
