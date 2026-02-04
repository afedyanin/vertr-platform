using Vertr.Common.Application.Abstractions;
using Vertr.Strategies.FuturesArbitrage.Models;

namespace Vertr.Strategies.FuturesArbitrage.EventHandlers;

internal sealed class BaseAssetOrderBookHandler : IEventHandler<OrderBookChangedEvent>
{
    public int HandlingOrder => 10;

    public ValueTask OnEvent(OrderBookChangedEvent data)
    {
        // В индикаторе нужно хранить историю, вынести в приватное поле 
        var sberIndicator = new OrderBookStatsInfo(2);
        sberIndicator.Apply(data.OrderBook);

        // если мид цена вышла за стат погрешность, включаем сигнал
        data.TradingDirection = sberIndicator.MidPriceSignal;

        return ValueTask.CompletedTask;
    }
}
