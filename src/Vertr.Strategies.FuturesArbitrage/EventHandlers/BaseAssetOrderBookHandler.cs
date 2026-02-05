using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;
using Vertr.Strategies.FuturesArbitrage.Models;

namespace Vertr.Strategies.FuturesArbitrage.EventHandlers;

internal sealed class BaseAssetOrderBookHandler : IEventHandler<OrderBookChangedEvent>
{
    private readonly ILogger<BaseAssetOrderBookHandler> _logger;

    // Хранение истории для статистики
    private readonly OrderBookStatsInfo _orderBookStats = new OrderBookStatsInfo(1);

    public int HandlingOrder => 10;

    public BaseAssetOrderBookHandler(ILogger<BaseAssetOrderBookHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask OnEvent(OrderBookChangedEvent data)
    {
        data.TradingDirection = _orderBookStats.UpdateAndGetDirection(data.OrderBook);

        if (data.TradingDirection != Common.Contracts.TradingDirection.Hold)
        {
            _logger.LogInformation("#{Sequence} Direction={Direction}", data.Sequence, data.TradingDirection);
        }

        return ValueTask.CompletedTask;
    }
}
