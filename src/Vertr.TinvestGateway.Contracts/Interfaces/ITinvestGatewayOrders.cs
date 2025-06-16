using Vertr.TinvestGateway.Contracts.Enums;

namespace Vertr.TinvestGateway.Contracts.Interfaces;
public interface ITinvestGatewayOrders
{
    public Task<Operation[]?> GetOperations(
        string accountId,
        DateTime? from = null,
        DateTime? to = null);

    public Task<PositionsResponse?> GetPositions(string accountId);

    public Task<PortfolioResponse?> GetPortfolio(string accountId);

    public Task<PostOrderResponse?> PostOrder(PostOrderRequest request);

    public Task<DateTime> CancelOrder(string accountId, string orderId);

    public Task<OrderState?> GetOrderState(
        string accountId,
        string orderId,
        PriceType priceType = PriceType.Unspecified);
}
