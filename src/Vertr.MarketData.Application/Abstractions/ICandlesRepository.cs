using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.Application.Abstractions;

public interface ICandlesRepository
{
    public Task<Candle[]> GetByQuery(CandlesQuery query);

    public Task<bool> Save(Candle candle);

    public Task<bool> Delete(
        string symbol,
        CandleInterval interval,
        CandleSource source);
}
