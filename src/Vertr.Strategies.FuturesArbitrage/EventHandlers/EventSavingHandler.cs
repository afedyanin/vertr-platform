using Vertr.Common.Application.Abstractions;

namespace Vertr.Strategies.FuturesArbitrage.EventHandlers;

internal sealed class EventSavingHandler : IEventHandler<OrderBookChangedEvent>
{
    private readonly IMarketDataEventLocalStorage _marketDataEventLocalStorage;

    public int HandlingOrder => 1010;

    public EventSavingHandler(IMarketDataEventLocalStorage marketDataEventLocalStorage)
    {
        _marketDataEventLocalStorage = marketDataEventLocalStorage;
    }

    public ValueTask OnEvent(OrderBookChangedEvent data)
    {
        if (data.OrderRequests.Any())
        {
            _marketDataEventLocalStorage.Save(data);
        }

        return ValueTask.CompletedTask;
    }
}
