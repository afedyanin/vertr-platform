using AutoMapper;

namespace Vertr.Adapters.Tinvest.Converters;

public class MoneyConverter : ITypeConverter<Domain.Money, Tinkoff.InvestApi.V1.MoneyValue>
{
    public Tinkoff.InvestApi.V1.MoneyValue Convert(
        Domain.Money source,
        Tinkoff.InvestApi.V1.MoneyValue destination,
        ResolutionContext context)
    {
        var tmp = ToGoogleType(source);

        return new Tinkoff.InvestApi.V1.MoneyValue()
        {
            Currency = tmp.CurrencyCode,
            Units = tmp.Units,
            Nano = tmp.Nanos,
        };
    }

    private static Google.Type.Money ToGoogleType(Domain.Money money)
        => new Google.Type.Money
        {
            CurrencyCode = money.Currency,
            DecimalValue = money.Value,
        };
}
