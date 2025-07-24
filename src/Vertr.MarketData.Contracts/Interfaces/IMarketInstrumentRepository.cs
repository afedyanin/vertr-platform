namespace Vertr.MarketData.Contracts.Interfaces;

public interface IMarketInstrumentRepository
{
    public Task<Instrument?> GetInstrument(Symbol symbol);

    public Task<Instrument?> GetInstrumentById(Guid instrumentId);

    public Task<Instrument[]> GetInstruments();

    public Task<Instrument[]> FindInstrument(string query);
}
