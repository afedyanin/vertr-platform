namespace Vertr.MarketData.Contracts.Interfaces;

public interface IMarketDataManager
{
    public Task<CandleInstrument[]> GetCandleSubscriptions();
}
