namespace Vertr.MarketData.Contracts.Interfaces;

public interface IMarketDataService
{
    public Task<CandleSubscription[]> GetSubscriptions();
}
