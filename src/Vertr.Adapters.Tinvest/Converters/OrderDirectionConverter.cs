namespace Vertr.Adapters.Tinvest.Converters;
internal static class OrderDirectionConverter
{
    public static Tinkoff.InvestApi.V1.OrderDirection Convert(this Domain.OrderDirection orderDirection)
        => orderDirection switch
        {
            Domain.OrderDirection.Unspecified => Tinkoff.InvestApi.V1.OrderDirection.Unspecified,
            Domain.OrderDirection.Buy => Tinkoff.InvestApi.V1.OrderDirection.Buy,
            Domain.OrderDirection.Sell => Tinkoff.InvestApi.V1.OrderDirection.Sell,
            _ => throw new InvalidOperationException($"Unknown order direction={orderDirection}"),
        };

    public static Domain.OrderDirection Convert(this Tinkoff.InvestApi.V1.OrderDirection orderDirection)
        => orderDirection switch
        {
            Tinkoff.InvestApi.V1.OrderDirection.Unspecified => Domain.OrderDirection.Unspecified,
            Tinkoff.InvestApi.V1.OrderDirection.Buy => Domain.OrderDirection.Buy,
            Tinkoff.InvestApi.V1.OrderDirection.Sell => Domain.OrderDirection.Sell,
            _ => throw new InvalidOperationException($"Unknown order direction={orderDirection}"),
        };
}
