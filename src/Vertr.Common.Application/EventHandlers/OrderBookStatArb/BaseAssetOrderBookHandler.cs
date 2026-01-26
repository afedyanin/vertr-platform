using Vertr.Common.Application.Abstractions;

namespace Vertr.Common.Application.EventHandlers.OrderBookStatArb;

internal class BaseAssetOrderBookHandler : IEventHandler<OrderBookChangedEvent>
{
    public ValueTask OnEvent(OrderBookChangedEvent data)
    {
        // Calculate stats
        throw new NotImplementedException();
    }
}
