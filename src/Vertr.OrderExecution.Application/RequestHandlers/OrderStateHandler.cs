using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.OrderExecution.Application.Abstractions;
using Vertr.OrderExecution.Application.Factories;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.PortfolioManager.Contracts.Requests;

namespace Vertr.OrderExecution.Application.RequestHandlers;

internal class OrderStateHandler : IRequestHandler<OrderStateRequest>
{
    private readonly IOrderEventRepository _orderEventRepository;
    private readonly ILogger<OrderStateHandler> _logger;
    private readonly IMediator _mediator;

    public OrderStateHandler(
        IOrderEventRepository orderEventRepository,
        IMediator mediator,
        ILogger<OrderStateHandler> logger)
    {
        _orderEventRepository = orderEventRepository;
        _mediator = mediator;
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

        // Сейчас вся необходимая информация приходит либо в OrderResponse, либо в OrderTrades
        // Для избежания дублирования событий, ничего не делаем
        return;

        _logger.LogInformation($"OrderState received: OrderId={orderState.OrderId} RequestId={orderState.OrderRequestId} AccountId={request.AccountId}");

        var portfolioId = await _orderEventRepository.ResolvePortfolioByOrderId(orderState.OrderId);

        if (portfolioId == null)
        {
            // Если не нашли portfolioId, значит ордер был выставлен в обход этого API
            _logger.LogWarning($"Cannot get portfolio identity for OrderId={orderState.OrderId}. Using OrderTrades.AccountId");

            if (string.IsNullOrEmpty(request.AccountId))
            {
                _logger.LogWarning($"Cannot get AccountId for OrderId={orderState.OrderId}. OrderTrades.AccountId is empty. Skipping message.");
                return;
            }

            portfolioId = new PortfolioIdentity(request.AccountId);
        }

        var orderEvent = orderState.CreateEvent(portfolioId);
        var saved = await _orderEventRepository.Save(orderEvent);

        if (!saved)
        {
            _logger.LogWarning($"Cannot save OrderState event for OrderId={orderEvent.OrderId}");
        }

        var operations = orderState.CreateOperations(portfolioId);

        var tradeOperationsRequest = new TradeOperationsRequest
        {
            Operations = operations,
        };

        _logger.LogDebug($"Publish OrderState operations for OrderId={orderState.OrderId}");
        await _mediator.Send(tradeOperationsRequest, cancellationToken);
    }
}
