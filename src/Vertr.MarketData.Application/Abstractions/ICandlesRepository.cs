namespace Vertr.MarketData.Application.Abstractions;

public interface ICandlesRepository
{
    public Task<bool> Save(Candle candle);

    public Task<bool> DeleteCandles(string symbol, CandleInterval interval, CandleSource source = CandleSource.None);
}
