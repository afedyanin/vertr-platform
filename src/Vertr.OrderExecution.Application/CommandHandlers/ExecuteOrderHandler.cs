using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vertr.OrderExecution.Application.Factories;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.OrderExecution.Contracts.Enums;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.Platform.Common.Channels;
using Vertr.Platform.Common.Mediator;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Application.RequestHandlers;

internal class ExecuteOrderHandler : IRequestHandler<ExecuteOrderRequest, ExecuteOrderResponse>
{
    private readonly IOrderExecutionGateway _executionGateway;
    private readonly IOrderExecutionSimulator _executionSimulator;
    private readonly IOrderEventRepository _orderEventRepository;
    private readonly IDataProducer<TradeOperation> _tradeOperationsProducer;
    private readonly ILogger<ExecuteOrderHandler> _logger;
    private readonly OrderExecutionSettings _orderExecutionSettings;

    public ExecuteOrderHandler(
        IDataProducer<TradeOperation> tradeOperationsProducer,
        IOrderExecutionGateway executionGateway,
        IOrderEventRepository orderEventRepository,
        IOrderExecutionSimulator executionSimulator,
        IOptions<OrderExecutionSettings> options,
        ILogger<ExecuteOrderHandler> logger)
    {
        _executionGateway = executionGateway;
        _executionSimulator = executionSimulator;
        _orderEventRepository = orderEventRepository;
        _tradeOperationsProducer = tradeOperationsProducer;
        _orderExecutionSettings = options.Value;
        _logger = logger;
    }

    public async Task<ExecuteOrderResponse> Handle(ExecuteOrderRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Posting new market order for SubAccountId={request.SubAccountId}");

        var orderId = await PostMarketOrder(
            request.RequestId,
            request.InstrumentId,
            request.SubAccountId,
            request.QtyLots,
            request.CreatedAt,
            request.BacktestId,
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
        Guid subAccountId,
        long qtyLots,
        DateTime createdAt,
        Guid? backtestId,
        CancellationToken cancellationToken)
    {
        var request = new PostOrderRequest
        {
            AccountId = _orderExecutionSettings.AccountId,
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

        var portfolioIdentity = new PortfolioIdentity(_orderExecutionSettings.AccountId, subAccountId);

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

        PostOrderResponse? response = null;

        if (_orderExecutionSettings.IsPaperTrading || backtestId.HasValue)
        {
            response = await _executionSimulator.PostOrder(request);
        }
        else
        {
            response = await _executionGateway.PostOrder(request);
        }

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
