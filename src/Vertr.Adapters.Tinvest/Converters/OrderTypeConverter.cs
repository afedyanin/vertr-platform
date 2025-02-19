namespace Vertr.Adapters.Tinvest.Converters;
internal static class OrderTypeConverter
{
    public static Tinkoff.InvestApi.V1.OrderType Convert(this Domain.OrderType orderType)
        => orderType switch
        {
            Domain.OrderType.Unspecified => Tinkoff.InvestApi.V1.OrderType.Unspecified,
            Domain.OrderType.Market => Tinkoff.InvestApi.V1.OrderType.Market,
            Domain.OrderType.Limit => Tinkoff.InvestApi.V1.OrderType.Limit,
            Domain.OrderType.Bestprice => Tinkoff.InvestApi.V1.OrderType.Bestprice,
            _ => throw new InvalidOperationException($"Unknown OrderType={orderType}"),
        };

    public static Domain.OrderType Convert(this Tinkoff.InvestApi.V1.OrderType orderType)
        => orderType switch
        {
            Tinkoff.InvestApi.V1.OrderType.Unspecified => Domain.OrderType.Unspecified,
            Tinkoff.InvestApi.V1.OrderType.Market => Domain.OrderType.Market,
            Tinkoff.InvestApi.V1.OrderType.Limit => Domain.OrderType.Limit,
            Tinkoff.InvestApi.V1.OrderType.Bestprice => Domain.OrderType.Bestprice,
            _ => throw new InvalidOperationException($"Unknown OrderType={orderType}"),
        };
}
