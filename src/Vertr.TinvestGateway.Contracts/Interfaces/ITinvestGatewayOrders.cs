using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Enums;

namespace Vertr.TinvestGateway.Contracts.Interfaces;
public interface ITinvestGatewayOrders
{
    public Task<PostOrderResponse?> PostOrder(PostOrderRequest request);

    public Task<DateTime> CancelOrder(string accountId, string orderId);

    public Task<OrderState?> GetOrderState(
        string accountId,
        string orderId,
        PriceType priceType = PriceType.Unspecified);
}
