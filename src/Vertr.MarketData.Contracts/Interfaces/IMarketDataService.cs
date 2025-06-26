namespace Vertr.MarketData.Contracts.Interfaces;

public interface IMarketDataService
{
    public Task Initialize();

    public Task<Instrument?> GetInstrument(InstrumentIdentity instrumentIdentity);

    public Task<CandleSubscription[]?> GetSubscriptions();
}
