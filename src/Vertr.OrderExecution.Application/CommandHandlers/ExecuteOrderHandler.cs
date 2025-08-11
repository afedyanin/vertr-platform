using Vertr.Platform.Common.Mediator;
using Microsoft.Extensions.Logging;
using Vertr.OrderExecution.Application.Factories;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.OrderExecution.Contracts.Enums;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.Platform.Common.Channels;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Application.RequestHandlers;

internal class ExecuteOrderHandler : IRequestHandler<ExecuteOrderCommand, ExecuteOrderResponse>
{
    private readonly IOrderExecutionGateway _tinvestGateway;
    private readonly IOrderEventRepository _orderEventRepository;
    private readonly IDataProducer<TradeOperation> _tradeOperationsProducer;
    private readonly ILogger<ExecuteOrderHandler> _logger;

    public ExecuteOrderHandler(
        IDataProducer<TradeOperation> tradeOperationsProducer,
        IOrderExecutionGateway tinvestGateway,
        IOrderEventRepository orderEventRepository,
        ILogger<ExecuteOrderHandler> logger)
    {
        _tinvestGateway = tinvestGateway;
        _orderEventRepository = orderEventRepository;
        _tradeOperationsProducer = tradeOperationsProducer;
        _logger = logger;
    }

    public async Task<ExecuteOrderResponse> Handle(ExecuteOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Posting new market order for PortfolioId={request.PortfolioIdentity}");

        var orderId = await PostMarketOrder(
            request.RequestId,
            request.InstrumentId,
            request.PortfolioIdentity,
            request.QtyLots,
            request.CreatedAt,
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
        PortfolioIdentity portfolioIdentity,
        long qtyLots,
        DateTime createdAt,
        CancellationToken cancellationToken)
    {
        var request = new PostOrderRequest
        {
            AccountId = portfolioIdentity.AccountId,
            RequestId = requestId,
            InstrumentId = instrumentId,
            OrderDirection = qtyLots > 0 ? OrderDirection.Buy : OrderDirection.Sell,
            Price = decimal.Zero,
            OrderType = OrderType.Market,
            PriceType = PriceType.Unspecified,
            TimeInForceType = TimeInForceType.Unspecified,
            QuantityLots = Math.Abs(qtyLots),
            CreatedAt = createdAt,
        };

        var postOrderEvent = request.CreateEvent(
            instrumentId,
            portfolioIdentity
            );

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

        var orderResponseEvent = response.CreateEvent(
            instrumentId,
            portfolioIdentity,
            request.CreatedAt);

        var savedResponse = await _orderEventRepository.Save(orderResponseEvent);

        if (!savedResponse)
        {
            _logger.LogError($"Cannot save order response. RequestId={requestId}");
        }

        var tradeOperations = response.CreateOperations(
            instrumentId,
            portfolioIdentity,
            request.CreatedAt);

        _logger.LogDebug($"Publish ExecuteOrder operations for OrderId={response.OrderId}");

        foreach (var tradeOperation in tradeOperations)
        {
            await _tradeOperationsProducer.Produce(tradeOperation, cancellationToken);
        }

        return response.OrderId;
    }
}
