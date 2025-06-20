using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;
using Vertr.PortfolioManager.Contracts;
using Vertr.TinvestGateway.Application.Converters;
using Vertr.TinvestGateway.Application.Settings;
using Vertr.TinvestGateway.Contracts.Interfaces;

namespace Vertr.TinvestGateway.Application.Proxy;

internal class TinvestGatewayAccounts : TinvestGatewayBase, ITinvestGatewayAccounts
{
    public TinvestGatewayAccounts(InvestApiClient investApiClient) : base(investApiClient)
    {
    }

    public async Task<PortfolioManager.Contracts.Account[]?> GetAccounts()
    {
        var response = await InvestApiClient.Users.GetAccountsAsync();
        var accounts = response.Accounts.ToArray().ToAccounts();

        return accounts;
    }
    public async Task<PortfolioManager.Contracts.Account[]?> GetSandboxAccounts()
    {
        var response = await InvestApiClient.Sandbox.GetSandboxAccountsAsync(new GetAccountsRequest());
        var accounts = response.Accounts.ToArray().ToAccounts();

        return accounts;
    }
    public async Task<string> CreateSandboxAccount(string accountName)
    {
        var tRequest = new OpenSandboxAccountRequest
        {
            Name = accountName,
        };

        var response = await InvestApiClient.Sandbox.OpenSandboxAccountAsync(tRequest);

        return response.AccountId;
    }

    public async Task CloseSandboxAccount(string accountId)
    {
        var tRequest = new CloseSandboxAccountRequest
        {
            AccountId = accountId,
        };

        _ = await InvestApiClient.Sandbox.CloseSandboxAccountAsync(tRequest);
    }

    public async Task<Money?> PayIn(string accountId, Money money)
    {
        var tRequest = new SandboxPayInRequest
        {
            AccountId = accountId,
            Amount = money.Convert()
        };

        var response = await InvestApiClient.Sandbox.SandboxPayInAsync(tRequest);
        var balance = response.Balance.Convert();

        return balance;
    }
}
