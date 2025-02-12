using Google.Protobuf.Collections;
using Vertr.Domain;

namespace Vertr.Adapters.Tinvest.Converters;

internal static class CandleConverter
{
    public static IEnumerable<HistoricCandle> Convert(this RepeatedField<Tinkoff.InvestApi.V1.HistoricCandle> candles)
    {
        foreach (var candle in candles)
        {
            yield return candle.Convert();
        }
    }

    public static HistoricCandle Convert(this Tinkoff.InvestApi.V1.HistoricCandle candle)
        => new HistoricCandle
        {
            TimeUtc = candle.Time.ToDateTime(),
            Open = candle.Open,
            Close = candle.Close,
            Low = candle.Low,
            High = candle.High,
            Volume = candle.Volume,
            IsCompleted = candle.IsComplete,
            CandleSource = (int)candle.CandleSource, // TODO: Use domain enum?
        };
}
