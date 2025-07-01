namespace Vertr.MarketData.Contracts.Interfaces;

public interface IStaticMarketDataProvider
{
    public Task<Instrument?> GetInstrument(Symbol symbol);

    public Task<Instrument?> GetInstrumentById(Guid instrumentId);

    public Task<Instrument[]> GetInstruments();

    public Task<Instrument[]> FindInstrument(string query);

    public Task<CandleSubscription[]> GetSubscriptions();

    public Task InitialLoad();

    public Task<Guid?> GetInstrumentCurrencyId(Guid instrumentId);

    public Guid? GetCurrencyId(string currencyCode);
}
