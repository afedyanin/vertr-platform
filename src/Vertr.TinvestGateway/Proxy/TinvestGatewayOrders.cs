using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.TinvestGateway.Abstractions;
using Vertr.TinvestGateway.Contracts.Orders;
using Vertr.TinvestGateway.Contracts.Orders.Enums;
using Vertr.TinvestGateway.Converters;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.Proxy;

internal class TinvestGatewayOrders : TinvestGatewayBase, IOrderExecutionGateway
{
    private readonly ILogger<TinvestGatewayOrders> _logger;

    private readonly IOrderRequestRepository _orderRequestRepository;
    private readonly IOrderResponseRepository _orderResponseRepository;
    private readonly IPortfolioService _portfolioService;
    private readonly TinvestSettings _tinvestSettings;

    public TinvestGatewayOrders(
        InvestApiClient investApiClient,
        IOrderRequestRepository orderRequestRepository,
        IOrderResponseRepository orderResponseRepository,
        IPortfolioService portfolioService,
        IOptions<TinvestSettings> tinvestSettings,
        ILogger<TinvestGatewayOrders> logger) : base(investApiClient)
    {
        _logger = logger;
        _orderRequestRepository = orderRequestRepository;
        _orderResponseRepository = orderResponseRepository;
        _portfolioService = portfolioService;
        _tinvestSettings = tinvestSettings.Value;
    }

    public async Task<PostOrderResponse?> PostOrder(PostOrderRequest request)
    {
        try
        {
            await _orderRequestRepository.Save(request, request.PortfolioId);
            var tRequest = request.Convert(_tinvestSettings.AccountId);
            var response = await InvestApiClient.Orders.PostOrderAsync(tRequest);
            var orderResponse = response.Convert();
            await _orderResponseRepository.Save(orderResponse);
            await _portfolioService.Update(orderResponse, request.PortfolioId);

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