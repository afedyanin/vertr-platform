namespace Vertr.CommandLine.Models.Abstracttions;

public interface IMarketDataService
{
    public Task<Candle[]> GetCandles(string symbol, DateTime before, int count = 1);

    public Task<decimal?> GetMarketPrice(string symbol, DateTime time, PriceType priceType, int shift = 0);

    public Task<IEnumerable<DateTime>> GetEnumerable(string symbol);

    public Task LoadData(string symbol, IEnumerable<Candle> candles);

    public Task<CandleRange?> GetCandleRange(string symbol);

    public Task<Dictionary<DateOnly, CandleRange>> GetCandleRanges(string symbol);
}