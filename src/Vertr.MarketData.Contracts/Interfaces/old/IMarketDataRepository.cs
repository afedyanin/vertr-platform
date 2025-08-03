namespace Vertr.MarketData.Contracts.Interfaces.old;

public interface IMarketDataRepository
{
    public void Add(Candle candle);

    public void AddRange(Candle[] candles);

    public Candle? GetLast(Guid instrumentId);

    public Candle[] GetAll(Guid instrumentId, int maxCount = 0);
}
