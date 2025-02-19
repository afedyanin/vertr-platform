using Google.Protobuf.Collections;
using Vertr.Domain;

namespace Vertr.Adapters.Tinvest.Converters;
internal static class AccountConverter
{
    public static IEnumerable<Account> Convert(this RepeatedField<Tinkoff.InvestApi.V1.Account> accounts)
    {
        foreach (var account in accounts)
        {
            yield return account.Convert();
        }
    }

    public static Account Convert(this Tinkoff.InvestApi.V1.Account account)
    {
        return new Account
        {
            Id = account.Id,
            Name = account.Name,
            AccessLevel = account.AccessLevel.ToString(),
            AccountType = account.Type.ToString(),
            Status = account.Status.ToString(),
            OpenedDate = account.OpenedDate.ToDateTime(),
            ClosedDate = account.ClosedDate.ToDateTime(),
        };
    }
}
