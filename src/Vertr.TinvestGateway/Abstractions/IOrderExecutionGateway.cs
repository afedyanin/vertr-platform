using Vertr.TinvestGateway.Contracts.Orders;
using Vertr.TinvestGateway.Contracts.Orders.Enums;

namespace Vertr.TinvestGateway.Abstractions;

public interface IOrderExecutionGateway
{
    public Task<PostOrderResponse?> PostOrder(PostOrderRequest request);

    public Task<DateTime?> CancelOrder(string accountId, string orderId);

    public Task<OrderState?> GetOrderState(
        string accountId,
        string orderId,
        PriceType priceType = PriceType.Unspecified);
}