namespace Vertr.MarketData.Contracts.Interfaces;

public interface IStaticMarketDataProvider
{
    public Task<Instrument?> GetInstrument(InstrumentIdentity instrumentIdentity);

    public Task<CandleInterval> GetInterval(InstrumentIdentity instrumentIdentity);

    public Task<Instrument[]> GetInstruments();

    public Task<Instrument[]> FindInstrument(string query);

    public Task<CandleSubscription[]> GetSubscriptions();

    public Task Load();
}
