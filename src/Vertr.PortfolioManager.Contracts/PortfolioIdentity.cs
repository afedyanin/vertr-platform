namespace Vertr.PortfolioManager.Contracts;

public record class PortfolioIdentity
{
    public string AccountId { get; private set; }

    public Guid SubAccountId { get; private set; }

    public PortfolioIdentity(string accountId, Guid? subAccountId = null)
    {
        AccountId = accountId;
        SubAccountId = subAccountId ?? Guid.Empty;
    }
}
