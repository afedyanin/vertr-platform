using Vertr.TinvestGateway.Contracts.Orders;

namespace Vertr.TinvestGateway.Abstractions;

public interface IPortfolioService
{
    public Task Update(OrderTrades orderTrades);

    public Task Update(PostOrderResponse orderResponse, Guid portfolioId);

    public Task BindOrderToPortfolio(OrderState orderState);
}