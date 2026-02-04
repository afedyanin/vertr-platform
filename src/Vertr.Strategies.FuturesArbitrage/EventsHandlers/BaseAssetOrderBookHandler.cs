using Vertr.Common.Application.Abstractions;

namespace Vertr.Strategies.FuturesArbitrage.EventsHandlers;

internal sealed class BaseAssetOrderBookHandler : IEventHandler<OrderBookChangedEvent>
{
    public int HandlingOrder => 10;

    public ValueTask OnEvent(OrderBookChangedEvent data)
    {
        // получение спот цены из стакана базового актива->Spot.MidPrice
        throw new NotImplementedException();
    }
}
