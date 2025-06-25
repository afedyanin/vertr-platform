using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.Application.Abstractions;
public interface IPortfolioRepository
{
    public Task<PortfolioSnapshot?> GetPortfolio(string accountId, Guid? bookId = null);

    public Task SetPortfolio(PortfolioSnapshot portfolio);

    public Task RemoveByAccountId(string accountId);

    public Task RemoveByBookId(Guid bookId);
}
