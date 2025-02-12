using Vertr.Domain;

namespace Vertr.Adapters.Tinvest.Converters;

internal static class CandleIntervalConverter
{
    public static Tinkoff.InvestApi.V1.CandleInterval Convert(this CandleInterval candleInterval)
    {
        return candleInterval switch
        {
            CandleInterval.Unspecified => Tinkoff.InvestApi.V1.CandleInterval.Unspecified,
            CandleInterval.Day1 => Tinkoff.InvestApi.V1.CandleInterval.Day,
            CandleInterval.Hour1 => Tinkoff.InvestApi.V1.CandleInterval.Hour,
            CandleInterval.Min15 => Tinkoff.InvestApi.V1.CandleInterval._15Min,
            CandleInterval.Min10 => Tinkoff.InvestApi.V1.CandleInterval._10Min,
            CandleInterval.Min5 => Tinkoff.InvestApi.V1.CandleInterval._5Min,
            CandleInterval.Min1 => Tinkoff.InvestApi.V1.CandleInterval._1Min,
            _ => throw new InvalidOperationException($"Unknown candle interval={candleInterval}"),
        };
    }
}
