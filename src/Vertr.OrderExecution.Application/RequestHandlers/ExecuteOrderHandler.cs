using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.MarketData.Contracts;
using Vertr.OrderExecution.Application.Factories;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Enums;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.PortfolioManager.Contracts.Requests;

namespace Vertr.OrderExecution.Application.RequestHandlers;

internal class PostOrderHandler : IRequestHandler<ExecuteOrderRequest, ExecuteOrderResponse>
{
    private readonly IOrderExecutionGateway _tinvestGateway;
    private readonly IOrderEventRepository _orderEventRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<PostOrderHandler> _logger;

    public PostOrderHandler(
        IOrderExecutionGateway tinvestGateway,
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
        _logger.LogInformation($"Posting new market order for PortfolioId={request.PortfolioIdentity}");

        var orderId = await PostMarketOrder(
            request.RequestId,
            request.InstrumentIdentity,
            request.PortfolioIdentity,
            request.QtyLots,
            cancellationToken);

        var response = new ExecuteOrderResponse
        {
            OrderId = orderId,
        };

        return response;
    }

    private async Task<string?> PostMarketOrder(
        Guid requestId,
        InstrumentIdentity instrumentIdentity,
        PortfolioIdentity portfolioIdentity,
        long qtyLots,
        CancellationToken cancellationToken)
    {
        var request = new PostOrderRequest
        {
            AccountId = portfolioIdentity.AccountId,
            RequestId = requestId,
            InstrumentIdentity = instrumentIdentity,
            OrderDirection = qtyLots > 0 ? OrderDirection.Buy : OrderDirection.Sell,
            Price = decimal.Zero,
            OrderType = OrderType.Market,
            PriceType = PriceType.Unspecified,
            TimeInForceType = TimeInForceType.Unspecified,
            QuantityLots = Math.Abs(qtyLots),
        };

        var postOrderEvent = request.CreateEvent(instrumentIdentity, portfolioIdentity);

        var savedRequest = await _orderEventRepository.Save(postOrderEvent);

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

        var orderResponseEvent = response.CreateEvent(instrumentIdentity, portfolioIdentity);

        var savedResponse = await _orderEventRepository.Save(orderResponseEvent);

        if (!savedResponse)
        {
            _logger.LogError($"Cannot save order response. RequestId={requestId}");
        }

        var tradeOperations = response.CreateOperations(instrumentIdentity, portfolioIdentity);

        var tradeOperationsRequest = new TradeOperationsRequest
        {
            Operations = tradeOperations,
        };

        _logger.LogDebug($"Publish ExecuteOrder operations for OrderId={response.OrderId}");
        await _mediator.Send(tradeOperationsRequest, cancellationToken);

        return response.OrderId;
    }
}
