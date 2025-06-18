using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.MarketData.Application;

internal class MarketDataService : IMarketDataService
{
    public Task<CandleSubscription[]> GetSubscriptions()
    {
        throw new NotImplementedException();
    }
}
