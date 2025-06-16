using Vertr.MarketData.Contracts;
using Vertr.TinvestGateway.Contracts.Enums;

namespace Vertr.MarketData.Converters;

internal static class CandleConverter
{
    public static Candle Convert(this TinvestGateway.Contracts.Candle source)
        => new Candle(
            Guid.NewGuid(),
            source.TimeUtc,
            source.Symbol,
            source.Interval.Convert(),
            CandleSource.Tinvest,
            source.Open,
            source.Close,
            source.High,
            source.Low,
            source.Volume);

    private static CandleInterval Convert(this TinvestGateway.Contracts.Enums.CandleInterval source)
        => source switch
        {
            TinvestGateway.Contracts.Enums.CandleInterval.Min_1 => CandleInterval.Min_1,
            TinvestGateway.Contracts.Enums.CandleInterval.Min_5 => CandleInterval.Min_5,
            TinvestGateway.Contracts.Enums.CandleInterval.Min_10 => CandleInterval.Min_10,
            TinvestGateway.Contracts.Enums.CandleInterval.Hour => CandleInterval.Hour,
            TinvestGateway.Contracts.Enums.CandleInterval.Day => CandleInterval.Day,
            TinvestGateway.Contracts.Enums.CandleInterval.Unspecified => CandleInterval.Unspecified,
            _ => throw new NotImplementedException(),
        };
}
