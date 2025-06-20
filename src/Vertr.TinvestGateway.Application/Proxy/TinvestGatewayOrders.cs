using Google.Protobuf.WellKnownTypes;
using Tinkoff.InvestApi;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Enums;
using Vertr.TinvestGateway.Application.Converters;
using Vertr.TinvestGateway.Contracts;
using Vertr.TinvestGateway.Contracts.Interfaces;

namespace Vertr.TinvestGateway.Application.Proxy;

internal class TinvestGatewayOrders : TinvestGatewayBase, ITinvestGatewayOrders
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

    public async Task<Operation[]?> GetOperations(string accountId, DateTime? from = null, DateTime? to = null)
    {
        var request = new Tinkoff.InvestApi.V1.OperationsRequest
        {
            AccountId = accountId,
        };

        if (from.HasValue)
        {
            request.From = Timestamp.FromDateTime(from.Value);
        }

        if (to.HasValue)
        {
            request.To = Timestamp.FromDateTime(to.Value);
        }

        var response = await InvestApiClient.Operations.GetOperationsAsync(request);
        var operations = response.Operations.ToArray().Convert();

        return operations;
    }


    public async Task<PortfolioResponse?> GetPortfolio(string accountId)
    {
        var request = new Tinkoff.InvestApi.V1.PortfolioRequest
        {
            AccountId = accountId,
            Currency = Tinkoff.InvestApi.V1.PortfolioRequest.Types.CurrencyRequest.Rub
        };

        var response = await InvestApiClient.Operations.GetPortfolioAsync(request);
        var portfolio = response.Convert();

        return portfolio;
    }

    public async Task<PositionsResponse?> GetPositions(string accountId)
    {
        var request = new Tinkoff.InvestApi.V1.PositionsRequest
        {
            AccountId = accountId,
        };

        var response = await InvestApiClient.Operations.GetPositionsAsync(request);
        var result = response.Convert(accountId);

        return result;
    }
}
