namespace Vertr.MarketData.Contracts.Interfaces;

public interface ICurrencyRepository
{
    public Task<Guid?> GetInstrumentCurrencyId(Guid instrumentId);

    public Task<string?> GetInstrumentCurrency(Guid instrumentId);

    public Task<Guid?> GetCurrencyId(string currencyCode);
}
