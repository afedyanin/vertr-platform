namespace Vertr.TinvestGateway.Repositories;

public interface IPortfolioOrdersRepository
{
    public Task BindOrderToPortfolio(string orderId, Guid portfolioId);

    public Task<Guid?> GetPortfolioByOrderId(string orderId);
}
