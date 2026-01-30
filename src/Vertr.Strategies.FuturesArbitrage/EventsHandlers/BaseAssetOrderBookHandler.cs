using Vertr.Common.Application.Abstractions;

namespace Vertr.Strategies.FuturesArbitrage.EventsHandlers;

internal sealed class BaseAssetOrderBookHandler : IEventHandler<OrderBookChangedEvent>
{
    public ValueTask OnEvent(OrderBookChangedEvent data)
    {
        // Calculate stats
        throw new NotImplementedException();
    }
}
