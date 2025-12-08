using Refit;
using Vertr.Common.Contracts;
using Vertr.TinvestGateway.Contracts.Orders;

namespace Vertr.TinvestGateway.Contracts;

public interface ITinvestGatewayClient
{
    [Get("/api/order-storage/portfolio/{portfolioId}")]
    public Task<Portfolio?> GetPortfolio(Guid portfolioId);

    [Post("/api/tinvest/orders")]
    public Task<PostOrderResponse> PostOrder(PostOrderRequest request);
}