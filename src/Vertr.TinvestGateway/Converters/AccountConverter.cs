using Vertr.TinvestGateway.Contracts.Portfolios;

namespace Vertr.TinvestGateway.Converters;

internal static class AccountConverter
{
    public static Account ToAccount(this Tinkoff.InvestApi.V1.Account tAccount)
        => new Account(
            tAccount.Id,
            tAccount.Name,
            tAccount.AccessLevel.ToString(),
            tAccount.Status.ToString(),
            tAccount.Type.ToString(),
            tAccount.OpenedDate.ToDateTime(),
            tAccount.ClosedDate?.ToDateTime());

    public static Account[] ToAccounts(this Tinkoff.InvestApi.V1.Account[] tAccounts)
        => [.. tAccounts.Select(ToAccount)];
}