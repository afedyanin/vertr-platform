using Vertr.PortfolioManager.Contracts;

namespace Vertr.TinvestGateway.Contracts.Interfaces;
public interface ITinvestGatewayAccounts
{
    public Task<Account[]?> GetAccounts();

    public Task<Account[]?> GetSandboxAccounts();

    public Task<string> CreateSandboxAccount(string accountName);

    public Task CloseSandboxAccount(string accountId);

    public Task<Money?> PayIn(string accountId, Money money);

    public Task<Operation[]?> GetOperations(
        string accountId,
        DateTime? from = null,
        DateTime? to = null);

    public Task<PositionsResponse?> GetPositions(string accountId);

    public Task<PortfolioSnapshot?> GetPortfolio(string accountId);
}
