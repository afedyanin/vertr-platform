using Vertr.Common.Application.Abstractions;

namespace Vertr.Common.Application.LocalStorage;

internal sealed class MarketDataEventLocalStorage : IMarketDataEventLocalStorage
{

    public IEnumerable<IMarketDataEvent> GetAll()
    {
        throw new NotImplementedException();
    }

    public IMarketDataEvent? GetBySequence(int sequenceId)
    {
        throw new NotImplementedException();
    }

    public void Save(IMarketDataEvent marketDataEvent)
    {
        throw new NotImplementedException();
    }
}
