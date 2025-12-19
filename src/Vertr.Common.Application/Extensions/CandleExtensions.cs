using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Extensions;

public static class CandleExtensions
{
    public static CandleRangeInfo? GetCandlesRange(this IEnumerable<Candle>? candles)
    {
        return candles == null || !candles.Any()
            ? null
            : new CandleRangeInfo
            {
                From = candles.First().TimeUtc,
                To = candles.Last().TimeUtc,
                Count = candles.Count()
            };
    }

    public static Dictionary<DateOnly, CandleRangeInfo> GetCandlesDayRanges(this IEnumerable<Candle>? candles)
    {
        return candles == null || !candles.Any()
            ? []
            : candles
           .GroupBy(c => DateOnly.FromDateTime(c.TimeUtc))
           .Select(group => new CandleRangeInfo
           {
               From = group.OrderBy(item => item.TimeUtc).First().TimeUtc,
               To = group.OrderByDescending(item => item.TimeUtc).First().TimeUtc,
               Count = group.Count()
           })
           .ToDictionary(c => DateOnly.FromDateTime(c.From));
    }
}
