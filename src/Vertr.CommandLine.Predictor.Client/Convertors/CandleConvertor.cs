using Vertr.CommandLine.Models;
using Vertr.CommandLine.Predictor.Client.Models;

namespace Vertr.CommandLine.Predictor.Client.Convertors;

internal static class CandleConvertor
{
    public static CandleRequest[] ToRequest(this IEnumerable<Candle> candles)
        => candles.Select(ToRequest).ToArray();

    public static CandleRequest ToRequest(this Candle candle)
        => new CandleRequest
        {
            TimeUtc = candle.TimeUtc,
            Open = candle.Open,
            Close = candle.Close,
            High = candle.High,
            Low = candle.Low,
            Volume = candle.Volume,
        };

}