namespace Vertr.PortfolioManager.Contracts.Interfaces;

public interface IPortfolioGateway
{
    public Task<Account[]?> GetAccounts();

    public Task<Account[]?> GetSandboxAccounts();

    public Task<string> CreateSandboxAccount(string accountName);

    public Task CloseSandboxAccount(string accountId);

    public Task<Money?> PayIn(string accountId, Money money);

    public Task<Portfolio?> GetPortfolio(string accountId);
}
