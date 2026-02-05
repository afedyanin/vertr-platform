using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;

namespace Vertr.Strategies.FuturesArbitrage.EventHandlers;

internal sealed class BaseAssetOrderBookHandler : IEventHandler<OrderBookChangedEvent>
{
    private readonly ILogger<BaseAssetOrderBookHandler> _logger;

    public int HandlingOrder => 10;

    public BaseAssetOrderBookHandler(ILogger<BaseAssetOrderBookHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask OnEvent(OrderBookChangedEvent data)
    {
        _logger.LogDebug("Processing event #{Sequence}", data.Sequence);

        // В индикаторе нужно хранить историю, вынести в приватное поле 
        //var sberIndicator = new OrderBookStatsInfo(2);
        //sberIndicator.Apply(data.OrderBook);

        // если мид цена вышла за стат погрешность, включаем сигнал
        //data.TradingDirection = sberIndicator.MidPriceSignal;

        return ValueTask.CompletedTask;
    }
}
