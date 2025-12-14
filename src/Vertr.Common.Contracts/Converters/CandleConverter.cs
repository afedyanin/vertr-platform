namespace Vertr.Common.Contracts.Converters;

public static class CandleConverter
{
    public static Candle ToCandle(this Candlestick candlestick, Guid instrumentId)
        => new Candle(instrumentId, candlestick.GetDateTime(), candlestick.Open, candlestick.Close, candlestick.High, candlestick.Low, candlestick.Volume);

    public static Candle[] ToCandles(this IEnumerable<Candlestick?> candlesticks, Guid instrumentId)
    {
        var res = new List<Candle>();

        foreach (var candlestick in candlesticks)
        {
            if (candlestick == null)
            {
                continue;
            }

            res.Add(candlestick.Value.ToCandle(instrumentId));
        }

        return [.. res];
    }
}