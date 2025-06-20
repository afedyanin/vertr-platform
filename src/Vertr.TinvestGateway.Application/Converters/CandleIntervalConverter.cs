using Vertr.MarketData.Contracts;

namespace Vertr.TinvestGateway.Application.Converters;

internal static class CandleIntervalConverter
{
    public static Tinkoff.InvestApi.V1.CandleInterval Convert(this CandleInterval source)
        => source switch
        {
            CandleInterval.Unspecified => Tinkoff.InvestApi.V1.CandleInterval.Unspecified,
            CandleInterval.Min_1 => Tinkoff.InvestApi.V1.CandleInterval._1Min,
            CandleInterval.Min_5 => Tinkoff.InvestApi.V1.CandleInterval._5Min,
            CandleInterval.Min_10 => Tinkoff.InvestApi.V1.CandleInterval._10Min,
            CandleInterval.Hour => Tinkoff.InvestApi.V1.CandleInterval.Hour,
            CandleInterval.Day => Tinkoff.InvestApi.V1.CandleInterval.Day,
            _ => throw new NotImplementedException(),
        };

    public static Tinkoff.InvestApi.V1.SubscriptionInterval ConvertToSubscriptionInterval(this CandleInterval source)
        => source switch
        {
            CandleInterval.Unspecified => Tinkoff.InvestApi.V1.SubscriptionInterval.Unspecified,
            CandleInterval.Min_1 => Tinkoff.InvestApi.V1.SubscriptionInterval.OneMinute,
            CandleInterval.Min_5 => Tinkoff.InvestApi.V1.SubscriptionInterval.FiveMinutes,
            CandleInterval.Min_10 => Tinkoff.InvestApi.V1.SubscriptionInterval._10Min,
            CandleInterval.Hour => Tinkoff.InvestApi.V1.SubscriptionInterval.OneHour,
            CandleInterval.Day => Tinkoff.InvestApi.V1.SubscriptionInterval.OneDay,
            _ => throw new NotImplementedException(),
        };
}
