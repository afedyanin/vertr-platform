using Refit;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Clients;

public interface ITinvestGatewayClient
{
    [Post("/api/tinvest/orders/market")]
    public Task PostMarketOrder(MarketOrderRequest request);

    // TODO: Implement this
    [Get("/api/tinvest/portfolios")]
    public Task<Portfolio[]> GetAllPortfolios(); // How to map to predictor??
}