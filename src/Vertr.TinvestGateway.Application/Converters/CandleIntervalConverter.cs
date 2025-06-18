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

    public static Tinkoff.InvestApi.V1.SubscriptionInterval ConvertToSubscriptionInterval(this MarketData.Contracts.CandleInterval source)
        => source switch
        {
            MarketData.Contracts.CandleInterval.Unspecified => Tinkoff.InvestApi.V1.SubscriptionInterval.Unspecified,
            MarketData.Contracts.CandleInterval.Min_1 => Tinkoff.InvestApi.V1.SubscriptionInterval.OneMinute,
            MarketData.Contracts.CandleInterval.Min_5 => Tinkoff.InvestApi.V1.SubscriptionInterval.FiveMinutes,
            MarketData.Contracts.CandleInterval.Min_10 => Tinkoff.InvestApi.V1.SubscriptionInterval._10Min,
            MarketData.Contracts.CandleInterval.Hour => Tinkoff.InvestApi.V1.SubscriptionInterval.OneHour,
            MarketData.Contracts.CandleInterval.Day => Tinkoff.InvestApi.V1.SubscriptionInterval.OneDay,
            _ => throw new NotImplementedException(),
        };
}
