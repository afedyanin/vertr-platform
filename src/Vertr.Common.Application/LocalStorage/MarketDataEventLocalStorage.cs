using Vertr.Common.Application.Abstractions;

namespace Vertr.Common.Application.LocalStorage;

internal sealed class MarketDataEventLocalStorage : IMarketDataEventLocalStorage
{
    private readonly Dictionary<int, IMarketDataEvent> _events = [];

    public IEnumerable<IMarketDataEvent> GetAll()
        => _events.Values;

    public IMarketDataEvent? GetBySequence(int sequenceId)
    {
        _events.TryGetValue(sequenceId, out var marketDataEvent);
        return marketDataEvent;
    }

    public void Save(IMarketDataEvent marketDataEvent)
    {
        _events[marketDataEvent.Sequence] = marketDataEvent;
    }
}
