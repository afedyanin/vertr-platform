using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.MarketData.Application.Internal;

internal class MarketDataManager : IMarketDataManager
{
    public Task<CandleInstrument[]> GetCandleSubscriptions()
    {
        throw new NotImplementedException();
    }
}
