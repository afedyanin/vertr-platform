namespace Vertr.MarketData.Contracts.Interfaces;

public interface IMarketDataInstrumentRepository
{
    public Task<Instrument[]> GetAll();

    public Task<Instrument?> GetBySymbol(Symbol symbol);

    public Task<Instrument?> GetById(Guid instrumentId);

    public Task<bool> Save(Instrument instrument);

    public Task<int> Delete(Guid Id);
}
