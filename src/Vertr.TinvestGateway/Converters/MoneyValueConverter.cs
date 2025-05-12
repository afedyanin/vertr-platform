using Tinkoff.InvestApi.V1;
using Vertr.TinvestGateway.Contracts;

namespace Vertr.TinvestGateway.Converters;

internal static class MoneyValueConverter
{
    public static MoneyValue ToMoneyValue(this Money money)
    {
        var tmp = ToGoogleType(money);

        var destination = new MoneyValue()
        {
            Currency = tmp.CurrencyCode,
            Units = tmp.Units,
            Nano = tmp.Nanos,
        };

        return destination;
    }

    public static Money FromMoneyValue(this MoneyValue moneyValue)
        => new Money(moneyValue, moneyValue.Currency);

    internal static Google.Type.Money ToGoogleType(Money money)
        => new Google.Type.Money
        {
            CurrencyCode = money.Currency,
            DecimalValue = money.Amount,
        };
}
