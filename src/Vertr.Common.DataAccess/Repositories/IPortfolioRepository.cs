using Vertr.Common.Contracts;

namespace Vertr.Common.DataAccess.Repositories;

public interface IPortfolioRepository
{
    public Task<Portfolio?> GetById(Guid portfolioId);

    public Task Save(Portfolio portfolio);

    public Task BindOrderToPortfolio(string orderId, Guid portfolioId);

    public Task<Guid?> GetPortfolioByOrderId(string orderId);
}