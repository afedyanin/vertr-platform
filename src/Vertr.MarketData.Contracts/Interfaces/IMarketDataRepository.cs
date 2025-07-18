namespace Vertr.MarketData.Contracts.Interfaces;

public interface IMarketDataRepository
{
    public void Add(Symbol symbol, CandleInterval interval, Candle candle);

    public void AddRange(Symbol symbol, CandleInterval interval, Candle[] candles);

    public Candle? GetLast(Symbol symbol, CandleInterval interval);

    public Candle[] GetAll(Symbol symbol, CandleInterval interval, int maxCount = 0);
}
