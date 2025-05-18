using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.Converters;

internal static class CandleConverter
{
    public static Candle Convert(this TinvestGateway.Contracts.Candle source)
        => new Candle(
            source.TimeUtc,
            source.Symbol,
            source.Interval.Convert(),
            CandleSource.Tinvest,
            source.Open,
            source.Close,
            source.High,
            source.Low,
            source.Volume);

    private static CandleInterval Convert(this TinvestGateway.Contracts.CandleInterval source)
        => source switch
        {
            TinvestGateway.Contracts.CandleInterval.Min_1 => CandleInterval.Min_1,
            TinvestGateway.Contracts.CandleInterval.Min_5 => CandleInterval.Min_5,
            TinvestGateway.Contracts.CandleInterval.Min_10 => CandleInterval.Min_10,
            TinvestGateway.Contracts.CandleInterval.Hour => CandleInterval.Hour,
            TinvestGateway.Contracts.CandleInterval.Day => CandleInterval.Day,
            TinvestGateway.Contracts.CandleInterval.Unspecified => CandleInterval.Unspecified,
            _ => throw new NotImplementedException(),
        };
}
