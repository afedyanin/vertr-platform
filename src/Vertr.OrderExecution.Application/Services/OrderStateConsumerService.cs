using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertr.OrderExecution.Application.Factories;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.Platform.Common;

namespace Vertr.OrderExecution.Application.Services;
internal class OrderStateConsumerService : DataConsumerServiceBase<OrderStateRequest>
{
    private readonly IOrderEventRepository _orderEventRepository;
    private readonly ILogger<OrderStateConsumerService> _logger;

    public OrderStateConsumerService(
        IServiceProvider serviceProvider,
        ILogger<OrderStateConsumerService> logger) : base(serviceProvider)
    {
        _logger = logger;
        _orderEventRepository = ServiceProvider.GetRequiredService<IOrderEventRepository>();
    }

    protected override async Task Handle(OrderStateRequest data, CancellationToken cancellationToken = default)
    {
        var orderState = data.OrderState;

        if (orderState == null)
        {
            _logger.LogWarning($"Empty OrderState received. Skipping message.");
            return;
        }

        _logger.LogInformation($"OrderState received: OrderId={orderState.OrderId} RequestId={orderState.OrderRequestId} AccountId={request.AccountId}");

        // TODO: Order State приходит раньше, чем сохраняется OrderResponse, поэтому не находит портфолио !!!

        var portfolioIdentity = await _orderEventRepository.ResolvePortfolioByOrderRequestId(Guid.Parse(orderState.OrderRequestId));

        if (portfolioIdentity == null)
        {
            // Если не нашли portfolioId, значит ордер был выставлен в обход этого API
            _logger.LogError($"Cannot get portfolio identity for OrderId={orderState.OrderId}.");
            return;
        }

        var orderEvent = orderState.CreateEvent(portfolioIdentity);
        var saved = await _orderEventRepository.Save(orderEvent);

        if (!saved)
        {
            _logger.LogWarning($"Cannot save OrderState event for OrderId={orderEvent.OrderId}");
            return;
        }

        // TODO: Нужно обрабатывать комиссии по новым трейдам, если они появляются

        /*
        var operations = orderState.CreateOperations(portfolioId);

        var tradeOperationsRequest = new TradeOperationsRequest
        {
            Operations = operations,
        };

        _logger.LogDebug($"Publish OrderState operations for OrderId={orderState.OrderId}");
        await _mediator.Send(tradeOperationsRequest, cancellationToken);
        */
    }
}
