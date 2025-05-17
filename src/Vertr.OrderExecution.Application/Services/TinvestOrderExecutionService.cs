using Microsoft.Extensions.Logging;
using Vertr.OrderExecution.Application.Abstractions;
using Vertr.OrderExecution.Application.Factories;
using Vertr.TinvestGateway.Contracts;

namespace Vertr.OrderExecution.Application.Services;

internal class TinvestOrderExecutionService : IOrderExecutionService
{
    private readonly ITinvestGateway _tinvestGateway;
    private readonly IOrderEventRepository _orderEventRepository;
    private readonly IOperationsPublisher _operationsPublisher;
    private readonly ILogger<TinvestOrderExecutionService> _logger;

    public TinvestOrderExecutionService(
        ITinvestGateway tinvestGateway,
        IOrderEventRepository orderEventRepository,
        IOperationsPublisher operationsPublisher,
        ILogger<TinvestOrderExecutionService> logger
        )
    {
        _tinvestGateway = tinvestGateway;
        _orderEventRepository = orderEventRepository;
        _operationsPublisher = operationsPublisher;
        _logger = logger;
    }

    public async Task<string?> PostMarketOrder(
        Guid requestId,
        Guid instrumentId,
        string accountId,
        long qtyLots,
        Guid bookId)
    {
        var request = new PostOrderRequest
        {
            AccountId = accountId,
            RequestId = requestId,
            InstrumentId = instrumentId,
            OrderDirection = qtyLots > 0 ? OrderDirection.Buy : OrderDirection.Sell,
            Price = decimal.Zero,
            OrderType = OrderType.Market,
            PriceType = PriceType.Unspecified,
            TimeInForceType = TimeInForceType.Unspecified,
            QuantityLots = Math.Abs(qtyLots),
        };

        var savedRequest = await _orderEventRepository.Save(request.CreateEvent(bookId));

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

        var savedResponse = await _orderEventRepository.Save(response.CreateEvent(bookId));

        if (!savedResponse)
        {
            _logger.LogError($"Cannot save order response. RequestId={requestId}");
        }

        var orderOperations = response.CreateOperations(accountId, bookId);
        await _operationsPublisher.Publish(orderOperations);

        return response.OrderId;
    }
}
