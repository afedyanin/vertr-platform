using Tinkoff.InvestApi.V1;
using Vertr.Domain;

namespace Vertr.Adapters.Tinvest.Converters;

internal static class MoneyConverter
{
    public static Money[] Convert(this IEnumerable<MoneyValue> moneyValues)
        => moneyValues.Select(Convert).ToArray();

    public static Money Convert(this MoneyValue moneyValue)
        => new Money(moneyValue.Currency, moneyValue);

    public static MoneyValue Convert(this Money money)
    {
        var tmp = money.ToGoogleType();

        return new MoneyValue()
        {
            Currency = tmp.CurrencyCode,
            Units = tmp.Units,
            Nano = tmp.Nanos,
        };
    }

    private static Google.Type.Money ToGoogleType(this Money money)
        => new Google.Type.Money
        {
            CurrencyCode = money.Currency,
            DecimalValue = money.Value,
        };
}
