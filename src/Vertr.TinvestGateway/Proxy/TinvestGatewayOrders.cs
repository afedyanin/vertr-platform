using Microsoft.Extensions.Logging;
using Tinkoff.InvestApi;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Enums;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.TinvestGateway.Converters;

namespace Vertr.TinvestGateway.Proxy;

internal class TinvestGatewayOrders : TinvestGatewayBase, IOrderExecutionGateway
{
    private ILogger<TinvestGatewayOrders> _logger;

    public TinvestGatewayOrders(
        InvestApiClient investApiClient,
        ILogger<TinvestGatewayOrders> logger) : base(investApiClient)
    {
        _logger = logger;
    }

    public async Task<PostOrderResponse?> PostOrder(PostOrderRequest request)
    {
        try
        {
            var tRequest = request.Convert();
            var response = await InvestApiClient.Orders.PostOrderAsync(tRequest);
            var orderResponse = response.Convert();

            return orderResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Post order failed: {ex.Message}");
        }

        return null;
    }

    public async Task<DateTime?> CancelOrder(string accountId, string orderId)
    {
        try
        {
            var cancelOrderRequest = new Tinkoff.InvestApi.V1.CancelOrderRequest
            {
                AccountId = accountId,
                OrderId = orderId,
            };

            var response = await InvestApiClient.Orders.CancelOrderAsync(cancelOrderRequest);

            return response.Time.ToDateTime();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Cancel order failed: {ex.Message}");
        }

        return null;
    }

    public async Task<OrderState?> GetOrderState(string accountId, string orderId, PriceType priceType = PriceType.Unspecified)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Get order state failed: {ex.Message}");
        }

        return null;
    }
}
