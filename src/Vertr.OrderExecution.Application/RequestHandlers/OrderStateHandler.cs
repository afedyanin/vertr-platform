using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.OrderExecution.Application.Abstractions;
using Vertr.OrderExecution.Application.Factories;
using Vertr.OrderExecution.Contracts.Requests;

namespace Vertr.OrderExecution.Application.RequestHandlers;

internal class OrderStateHandler : IRequestHandler<OrderStateRequest>
{
    private readonly IOrderEventRepository _orderEventRepository;
    private readonly ILogger<OrderStateHandler> _logger;

    public OrderStateHandler(
        IOrderEventRepository orderEventRepository,
        ILogger<OrderStateHandler> logger)
    {
        _orderEventRepository = orderEventRepository;
        _logger = logger;
    }

    public async Task Handle(OrderStateRequest request, CancellationToken cancellationToken)
    {
        var orderState = request.OrderState;

        if (orderState == null)
        {
            _logger.LogWarning($"Empty OrderState received. Skipping message.");
            return;
        }

        _logger.LogDebug($"OrderState received: {orderState}");

        var portfolioId = await _orderEventRepository.GetPortfolioIdByOrderId(orderState.OrderId);

        if (portfolioId == null)
        {
            // Если не нашли portfolioId, значит ордер был выставлен в обход этого API
            // TODO: Использовать OrderState.AccountId, когда он появится в SDK. См. обработку OrderTrades
            _logger.LogWarning($"Cannot get portfolio identity for OrderId={orderState.OrderId}. Skipping message.");
            return;
        }

        var orderEvent = orderState.CreateEvent(portfolioId);
        var saved = await _orderEventRepository.Save(orderEvent);

        if (!saved)
        {
            _logger.LogWarning($"Cannot save OrderState event for OrderId={orderEvent.OrderId}");
        }

        var operations = orderState.CreateOperations(portfolioId);

        _logger.LogDebug($"Publish OrderState operations for OrderId={orderState.OrderId}");
        await OperationsPublisher.Publish(operations);
    }
}
