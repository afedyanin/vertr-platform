using Refit;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Clients;

public interface ITinvestGatewayClient
{
    [Get("/api/order-storage/portfolio/{portfolioId}")]
    public Task<Portfolio?> GetPortfolio(Guid portfolioId);

    [Post("/api/tinvest/orders")]
    public Task PostOrder(OrderRequest request);
}