using AutoMapper;

namespace Vertr.Adapters.Tinvest.Converters;

public class MoneyConverter :
    ITypeConverter<Domain.Money, Tinkoff.InvestApi.V1.MoneyValue>,
    ITypeConverter<Tinkoff.InvestApi.V1.MoneyValue, Domain.Money>
{
    public Tinkoff.InvestApi.V1.MoneyValue Convert(
        Domain.Money source,
        Tinkoff.InvestApi.V1.MoneyValue destination,
        ResolutionContext context)
    {
        var tmp = ToGoogleType(source);

        destination = new Tinkoff.InvestApi.V1.MoneyValue()
        {
            Currency = tmp.CurrencyCode,
            Units = tmp.Units,
            Nano = tmp.Nanos,
        };

        return destination;
    }

    public Domain.Money Convert(
        Tinkoff.InvestApi.V1.MoneyValue source,
        Domain.Money destination,
        ResolutionContext context)
    {
        destination = new Domain.Money()
        {
            Currency = source.Currency,
            Value = source,
        };

        return destination;
    }

    internal static Google.Type.Money ToGoogleType(Domain.Money money)
        => new Google.Type.Money
        {
            CurrencyCode = money.Currency,
            DecimalValue = money.Value,
        };
}
