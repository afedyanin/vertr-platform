using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.OrderExecution.Application.Factories;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.PortfolioManager.Contracts.Requests;

namespace Vertr.OrderExecution.Application.RequestHandlers;

internal class OrderTradesHandler : IRequestHandler<OrderTradesRequest>
{
    private readonly IOrderEventRepository _orderEventRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<OrderTradesHandler> _logger;

    public OrderTradesHandler(
        IOrderEventRepository orderEventRepository,
        IMediator mediator,
        ILogger<OrderTradesHandler> logger)
    {
        _orderEventRepository = orderEventRepository;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(OrderTradesRequest request, CancellationToken cancellationToken)
    {
        var orderTrades = request.OrderTrades;

        _logger.LogDebug($"OrderTrades received: {orderTrades}");

        // TODO: OrderTrades может прийти раньше, чем сохранится OrderResponse, поэтому может не найти портфолио !!!
        // TODO: Нужна общая очередь для обработки респонсов по ордерам
        var portfolioIdentity = await _orderEventRepository.ResolvePortfolioByOrderId(orderTrades.OrderId);

        if (portfolioIdentity == null)
        {
            // Если не нашли portfolioId, значит ордер был выставлен в обход этого API
            _logger.LogError($"Cannot get portfolio identity for OrderId={orderTrades.OrderId}.");
            return;
        }

        var orderEvent = orderTrades.CreateEvent(request.InstrumentId, portfolioIdentity);
        var saved = await _orderEventRepository.Save(orderEvent);

        if (!saved)
        {
            _logger.LogWarning($"Cannot save OrderTrades event for OrderId={orderEvent.OrderId}");
            return;
        }

        var operations = orderTrades.CreateOperations(request.InstrumentId, portfolioIdentity);

        var tradeOperationsRequest = new TradeOperationsRequest
        {
            Operations = operations,
        };

        _logger.LogDebug($"Publish OrderTrades operations for OrderId={orderTrades.OrderId}");
        await _mediator.Send(tradeOperationsRequest, cancellationToken);
    }
}
