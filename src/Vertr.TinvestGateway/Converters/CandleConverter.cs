using Vertr.Common.Contracts;
using Vertr.TinvestGateway.Models;

namespace Vertr.TinvestGateway.Converters;

public static class CandleConverter
{
    public static Candlestick[] ToCandlesticks(this Tinkoff.InvestApi.V1.HistoricCandle[] source)
        => [.. source.Select(ToCandlestick)];

    public static Candlestick ToCandlestick(this Tinkoff.InvestApi.V1.HistoricCandle source)
        => new Candlestick(
            source.Time.ToDateTime(),
            source.High,
            source.Low,
            source.Open,
            source.Close,
            source.Volume);

    public static Candlestick ToCandlestick(
        this Tinkoff.InvestApi.V1.Candle source)
        => new Candlestick(
            source.Time.ToDateTime(),
            source.High,
            source.Low,
            source.Open,
            source.Close,
            source.Volume);

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