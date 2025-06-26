namespace Vertr.MarketData.Contracts.Interfaces;

internal interface IStaticMarketDataRepository
{
    public Task<Instrument?> GetInstrument(InstrumentIdentity instrumentIdentity);

    public Task<CandleInterval?> GetInterval(InstrumentIdentity instrumentIdentity);

    public Task<Instrument[]?> GetInstruments();

    public Task<CandleSubscription[]?> GetSubscriptions();

    public Task Save(Instrument[] instruments);

    public Task Clear();
}
