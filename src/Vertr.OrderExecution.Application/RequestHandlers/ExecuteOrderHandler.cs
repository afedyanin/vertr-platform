using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.OrderExecution.Application.Abstractions;
using Vertr.OrderExecution.Application.Factories;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Enums;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.PortfolioManager.Contracts.Requests;
using Vertr.TinvestGateway.Contracts;
using Vertr.TinvestGateway.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.RequestHandlers;

internal class PostOrderHandler : IRequestHandler<ExecuteOrderRequest, ExecuteOrderResponse>
{
    private readonly ITinvestGatewayOrders _tinvestGateway;
    private readonly IOrderEventRepository _orderEventRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<PostOrderHandler> _logger;

    public PostOrderHandler(
        ITinvestGatewayOrders tinvestGateway,
        IOrderEventRepository orderEventRepository,
        IMediator mediator,
        ILogger<PostOrderHandler> logger)
    {
        _tinvestGateway = tinvestGateway;
        _orderEventRepository = orderEventRepository;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<ExecuteOrderResponse> Handle(ExecuteOrderRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Posting new market order for PortfolioId={request.PortfolioId}");

        var orderId = await PostMarketOrder(
            request.RequestId,
            request.InstrumentId,
            request.QtyLots,
            request.PortfolioId,
            cancellationToken);

        var response = new ExecuteOrderResponse
        {
            OrderId = orderId,
        };

        return response;
    }

    private async Task<string?> PostMarketOrder(
        Guid requestId,
        Guid instrumentId,
        long qtyLots,
        PortfolioIdentity portfolioId,
        CancellationToken cancellationToken)
    {
        var request = new PostOrderRequest
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

        var tradeOperations = response.CreateOperations(portfolioId);

        var tradeOperationsRequest = new TradeOperationsRequest
        {
            Operations = tradeOperations,
        };

        _logger.LogDebug($"Publish ExecuteOrder operations for OrderId={response.OrderId}");
        await _mediator.Send(tradeOperationsRequest, cancellationToken);

        return response.OrderId;
    }

}
