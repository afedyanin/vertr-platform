using Vertr.Common.Contracts;

namespace Vertr.TinvestGateway.Repositories;

public interface IPortfolioRepository
{
    public Task<Portfolio?> GetById(Guid portfolioId);

    public Task Save(Portfolio portfolio);

    public Task BindOrderToPortfolio(string orderId, Guid portfolioId);

    public Task<Guid?> GetPortfolioByOrderId(string orderId);
}