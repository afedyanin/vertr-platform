using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.Application.Abstractions;

public interface ICandlesRepository
{
    public Task<Candle?> GetById(Guid id);

    public Task<Candle[]> GetByQuery(CandlesQuery query);

    public Task<bool> Save(Candle candle);

    public Task<bool> Delete(Guid id);

    public Task<bool> DeleteMany(
        string symbol,
        CandleInterval interval,
        CandleSource source);
}
