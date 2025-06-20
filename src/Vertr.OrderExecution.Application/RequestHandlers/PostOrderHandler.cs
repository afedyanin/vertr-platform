using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.OrderExecution.Application.Abstractions;
using Vertr.OrderExecution.Application.Factories;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.TinvestGateway.Contracts.Enums;
using Vertr.TinvestGateway.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.RequestHandlers;

internal class PostOrderHandler : IRequestHandler<PostOrderRequest, OrderExecutionResponse>
{
    private readonly ITinvestGatewayOrders _tinvestGateway;
    private readonly IOrderEventRepository _orderEventRepository;
    private readonly ILogger<PostOrderHandler> _logger;

    public PostOrderHandler(
        ITinvestGatewayOrders tinvestGateway,
        IOrderEventRepository orderEventRepository,
        ILogger<PostOrderHandler> logger)
    {
        _tinvestGateway = tinvestGateway;
        _orderEventRepository = orderEventRepository;
        _logger = logger;
    }

    public async Task<OrderExecutionResponse> Handle(PostOrderRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Posting new market order for PortfolioId={request.PortfolioId}");

        var orderId = await PostMarketOrder(
            request.RequestId,
            request.InstrumentId,
            request.QtyLots,
            request.PortfolioId);

        var response = new OrderExecutionResponse
        {
            OrderId = orderId,
        };

        return response;
    }

    public async Task<string?> PostMarketOrder(
        Guid requestId,
        Guid instrumentId,
        long qtyLots,
        PortfolioIdentity portfolioId)
    {
        var request = new TinvestGateway.Contracts.PostOrderRequest
        {
            AccountId = portfolioId.AccountId,
            RequestId = requestId,
            InstrumentId = instrumentId,
            OrderDirection = qtyLots > 0 ? OrderDirection.Buy : OrderDirection.Sell,
            Price = decimal.Zero,
            OrderType = OrderType.Market,
            PriceType = PriceType.Unspecified,
            TimeInForceType = TimeInForceType.Unspecified,
            QuantityLots = Math.Abs(qtyLots),
        };

        var savedRequest = await _orderEventRepository.Save(request.CreateEvent(portfolioId));

        if (!savedRequest)
        {
            _logger.LogError($"Cannot save order request. RequestId={requestId}");
            return null;
        }

        _logger.LogInformation($"Posting new market order. RequestId={requestId}");
        var response = await _tinvestGateway.PostOrder(request);

        if (response == null)
        {
            _logger.LogError($"Could not receive order response. RequestId={requestId}");
            return null;
        }

        var savedResponse = await _orderEventRepository.Save(response.CreateEvent(portfolioId));

        if (!savedResponse)
        {
            _logger.LogError($"Cannot save order response. RequestId={requestId}");
        }

        var orderOperations = response.CreateOperations(portfolioId);
        await _operationsPublisher.Publish(orderOperations);

        return response.OrderId;
    }

}
