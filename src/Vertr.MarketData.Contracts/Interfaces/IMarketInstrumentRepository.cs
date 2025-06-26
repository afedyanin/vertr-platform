namespace Vertr.MarketData.Contracts.Interfaces;

internal interface IMarketInstrumentRepository
{
    public Task Save(Instrument[] instruments);

    public Task<Instrument?> Get(InstrumentIdentity instrumentIdentity);

    public Task<Instrument[]> GetAll();

    public Task Clear();
}
