using Tinkoff.InvestApi;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Enums;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.TinvestGateway.Converters;

namespace Vertr.TinvestGateway.Proxy;

internal class TinvestGatewayOrders : TinvestGatewayBase, IOrderExecutionGateway
{
    public TinvestGatewayOrders(InvestApiClient investApiClient) : base(investApiClient)
    {
    }

    public async Task<PostOrderResponse?> PostOrder(PostOrderRequest request)
    {
        var tRequest = request.Convert();
        var response = await InvestApiClient.Orders.PostOrderAsync(tRequest);
        var orderResponse = response.Convert();

        return orderResponse;
    }

    public async Task<DateTime> CancelOrder(string accountId, string orderId)
    {
        var cancelOrderRequest = new Tinkoff.InvestApi.V1.CancelOrderRequest
        {
            AccountId = accountId,
            OrderId = orderId,
        };

        var response = await InvestApiClient.Orders.CancelOrderAsync(cancelOrderRequest);

        return response.Time.ToDateTime();
    }

    public async Task<OrderState?> GetOrderState(string accountId, string orderId, PriceType priceType = PriceType.Unspecified)
    {
        var orderStateRequest = new Tinkoff.InvestApi.V1.GetOrderStateRequest
        {
            AccountId = accountId,
            OrderId = orderId,
            PriceType = priceType.Convert(),
        };

        var response = await InvestApiClient.Orders.GetOrderStateAsync(orderStateRequest);
        var state = response.Convert();

        return state;
    }
}
