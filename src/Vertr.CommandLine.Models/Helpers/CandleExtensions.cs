namespace Vertr.CommandLine.Models.Helpers;

public static class CandleExtensions
{
    public static decimal? GetPrice(this Candle? candle, PriceType priceType)
    {
        return candle == null
            ? null
            : priceType switch
            {
                PriceType.Open => candle.Open,
                PriceType.Close => candle.Close,
                PriceType.Mid => (candle.Open + candle.Close) / 2,
                PriceType.Avg => (candle.Open + candle.Close + candle.Low + candle.High) / 4,
                _ => null,
            };
    }

    public static IEnumerable<DateTime> GetTimeEnumerable(
        this IEnumerable<Candle>? orderedCandles)
    {
        return orderedCandles == null ? [] : orderedCandles.Select(c => c.TimeUtc);
    }

    public static IEnumerable<Candle> GetEqualOrLessThanBefore(this IEnumerable<Candle> orderedCandles, DateTime before, int count = 1)
        => orderedCandles
            .Where(c => c.TimeUtc <= before)
            .TakeLast(count);

    public static IEnumerable<Candle> GetEqualOrGreatherThanAfter(this IEnumerable<Candle> orderedCandles, DateTime after, int count = 1)
        => orderedCandles
            .Where(c => c.TimeUtc >= after)
            .Take(count);

    public static Candle? GetShifted(this IEnumerable<Candle> orderedCandles, DateTime time, int shift = 0)
    {
        var count = Math.Abs(shift) + 1;

        var filtered = shift <= 0 ? [.. orderedCandles.GetEqualOrLessThanBefore(time, count)] :
            orderedCandles.GetEqualOrGreatherThanAfter(time, count).ToArray();

        // shift is out of range
        return filtered.Length < count ? null : shift <= 0 ? filtered.First() : filtered.Last();
    }

    public static CandleRange? GetRange(this IEnumerable<Candle>? candles, string symbol)
    {
        return candles == null || !candles.Any()
            ? null
            : new CandleRange
            {
                Symbol = symbol,
                FirstDate = candles.First().TimeUtc,
                LastDate = candles.Last().TimeUtc,
                Count = candles.Count()
            };
    }

    public static Dictionary<DateOnly, CandleRange> GetRanges(this IEnumerable<Candle>? candles, string symbol)
    {
        return candles == null || !candles.Any()
            ? []
            : candles
           .GroupBy(c => DateOnly.FromDateTime(c.TimeUtc))
           .Select(group => new CandleRange
           {
               Symbol = symbol,
               FirstDate = group.OrderBy(item => item.TimeUtc).First().TimeUtc,
               LastDate = group.OrderByDescending(item => item.TimeUtc).First().TimeUtc,
               Count = group.Count()
           })
           .ToDictionary(c => DateOnly.FromDateTime(c.FirstDate));
    }

    public static Candle? LastCandle(this IEnumerable<Candle>? candles)
    {
        return candles?.OrderBy(c => c.TimeUtc).LastOrDefault();
    }
}