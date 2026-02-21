using Vertr.Common.Application.Abstractions;

namespace Vertr.Strategies.FuturesArbitrage.EventHandlers;

internal sealed class EventSavingHandler : IEventHandler<OrderBookChangedEvent>
{
    public int HandlingOrder => 1010;

    public ValueTask OnEvent(OrderBookChangedEvent data)
    {
        throw new NotImplementedException();
    }
}
