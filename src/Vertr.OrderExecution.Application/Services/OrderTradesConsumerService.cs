using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertr.OrderExecution.Application.Factories;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.Platform.Common;
using Vertr.PortfolioManager.Contracts.Requests;

namespace Vertr.OrderExecution.Application.Services;

internal class OrderTradesConsumerService : DataConsumerServiceBase<OrderTradesRequest>
{
    private readonly IOrderEventRepository _orderEventRepository;
    private readonly ILogger<OrderTradesConsumerService> _logger;

    public OrderTradesConsumerService(
        IServiceProvider serviceProvider,
        ILogger<OrderTradesConsumerService> logger) : base(serviceProvider)
    {
        _logger = logger;
        _orderEventRepository = ServiceProvider.GetRequiredService<IOrderEventRepository>();
    }

    protected override async Task Handle(OrderTradesRequest data, CancellationToken cancellationToken = default)
    {
        var orderTrades = data.OrderTrades;

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

        var orderEvent = orderTrades.CreateEvent(data.InstrumentId, portfolioIdentity);
        var saved = await _orderEventRepository.Save(orderEvent);

        if (!saved)
        {
            _logger.LogWarning($"Cannot save OrderTrades event for OrderId={orderEvent.OrderId}");
            return;
        }

        var operations = orderTrades.CreateOperations(data.InstrumentId, portfolioIdentity);

        var tradeOperationsRequest = new TradeOperationsRequest
        {
            Operations = operations,
        };

        _logger.LogDebug($"Publish OrderTrades operations for OrderId={orderTrades.OrderId}");
        await _mediator.Send(tradeOperationsRequest, cancellationToken);
    }
}
