using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertr.OrderExecution.Application.Factories;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.Platform.Common.Channels;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Application.Services;

internal class OrderTradesConsumerService : DataConsumerServiceBase<OrderTrades>
{
    private readonly IOrderEventRepository _orderEventRepository;
    private readonly IDataProducer<TradeOperation> _tradeOperationsProducer;

    private readonly ILogger<OrderTradesConsumerService> _logger;

    public OrderTradesConsumerService(
        IServiceProvider serviceProvider,
        ILogger<OrderTradesConsumerService> logger) : base(serviceProvider)
    {
        _logger = logger;
        _orderEventRepository = ServiceProvider.GetRequiredService<IOrderEventRepository>();
        _tradeOperationsProducer = ServiceProvider.GetRequiredService<IDataProducer<TradeOperation>>();
    }

    protected override async Task Handle(OrderTrades data, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug($"OrderTrades received: {data}");

        // TODO: OrderTrades может прийти раньше, чем сохранится OrderResponse, поэтому может не найти портфолио !!!
        // TODO: Нужна общая очередь для обработки респонсов по ордерам
        var portfolioIdentity = await _orderEventRepository.ResolvePortfolioByOrderId(data.OrderId);

        if (portfolioIdentity == null)
        {
            // Если не нашли portfolioId, значит ордер был выставлен в обход этого API
            _logger.LogError($"Cannot get portfolio identity for OrderId={data.OrderId}.");
            return;
        }

        var orderEvent = data.CreateEvent(data.InstrumentId, portfolioIdentity);
        var saved = await _orderEventRepository.Save(orderEvent);

        if (!saved)
        {
            _logger.LogWarning($"Cannot save OrderTrades event for OrderId={orderEvent.OrderId}");
            return;
        }

        var operations = data.CreateOperations(data.InstrumentId, portfolioIdentity);

        _logger.LogDebug($"Publish OrderTrades operations for OrderId={data.OrderId}");

        foreach (var operation in operations)
        {
            await _tradeOperationsProducer.Produce(operation, cancellationToken);

        }
    }
}
