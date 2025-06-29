using System.Text.Json.Serialization;

namespace Vertr.PortfolioManager.Contracts;

public record class PortfolioIdentity
{
    public string AccountId { get; private set; }

    public Guid SubAccountId { get; private set; }

    [JsonConstructor]
    public PortfolioIdentity()
    {
    }

    public PortfolioIdentity(string accountId, Guid? subAccountId = null)
    {
        AccountId = accountId;
        SubAccountId = subAccountId ?? Guid.Empty;
    }
}
