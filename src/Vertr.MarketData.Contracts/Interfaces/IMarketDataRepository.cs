namespace Vertr.MarketData.Contracts.Interfaces;

public interface IMarketDataRepository
{
    public Candle GetLast(Instrument instrument);

    public Candle[] GetAll(Instrument instrument, int maxItems = 100);

    public void Save(Instrument instrument, Candle[] candles);

    public void DeleteAll(Instrument instrument);

    public void DeleteFirst(Instrument instrument);

    public Task InitialLoad();
}
