using Vertr.PortfolioManager.Application.Abstractions;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.DataAccess.Repositories;

internal class PortfolioRepository : IPortfolioRepository
{
    public Task<PortfolioSnapshot?> GetPortfolio(string accountId, Guid? bookId = null)
    {
        throw new NotImplementedException();
    }

    public Task RemoveByAccountId(string accountId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveByBookId(Guid bookId)
    {
        throw new NotImplementedException();
    }

    public Task SetPortfolio(PortfolioSnapshot portfolio)
    {
        throw new NotImplementedException();
    }
}
