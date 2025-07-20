using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.Application.Tests;

internal static class CandleFactory
{
    public static Candle[] CreateCandles(DateTime startDate, int count)
    {
        var candles = new List<Candle>();
        var time = startDate;
        var instrumentId = Guid.NewGuid();

        for (int i = 0; i < count; i++)
        {
            var candle = new Candle(time, 10.1m, 10.2m, 10.3m, 10.4m, 100, instrumentId);
            candles.Add(candle);
            time = time.AddMinutes(1);
        }

        return [.. candles];
    }
}
