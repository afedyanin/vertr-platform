using Vertr.TinvestGateway.Contracts;

namespace Vertr.TinvestGateway.Converters;

internal static class CandleIntervalConverter
{
    public static Tinkoff.InvestApi.V1.CandleInterval Convert(this CandleInterval interval)
        => interval switch
        {
            CandleInterval.Unspecified => Tinkoff.InvestApi.V1.CandleInterval.Unspecified,
            CandleInterval.Min_1 => Tinkoff.InvestApi.V1.CandleInterval._1Min,
            CandleInterval.Min_5 => Tinkoff.InvestApi.V1.CandleInterval._5Min,
            CandleInterval.Min_10 => Tinkoff.InvestApi.V1.CandleInterval._10Min,
            CandleInterval.Hour => Tinkoff.InvestApi.V1.CandleInterval.Hour,
            CandleInterval.Day => Tinkoff.InvestApi.V1.CandleInterval.Day,
            _ => throw new NotImplementedException(),
        };
}
