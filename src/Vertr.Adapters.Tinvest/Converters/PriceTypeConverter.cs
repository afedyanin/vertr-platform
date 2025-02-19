namespace Vertr.Adapters.Tinvest.Converters;

internal static class PriceTypeConverter
{
    public static Tinkoff.InvestApi.V1.PriceType Convert(this Domain.PriceType priceType)
        => priceType switch
        {
            Domain.PriceType.Unspecified => Tinkoff.InvestApi.V1.PriceType.Unspecified,
            Domain.PriceType.Point => Tinkoff.InvestApi.V1.PriceType.Point,
            Domain.PriceType.Currency => Tinkoff.InvestApi.V1.PriceType.Currency,
            _ => throw new InvalidOperationException($"Unknown PriceType={priceType}"),
        };
}
