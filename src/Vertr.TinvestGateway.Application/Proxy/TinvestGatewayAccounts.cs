using Google.Protobuf.WellKnownTypes;
using Tinkoff.InvestApi;
using Vertr.PortfolioManager.Contracts;
using Vertr.TinvestGateway.Application.Converters;
using Vertr.TinvestGateway.Contracts;
using Vertr.TinvestGateway.Contracts.Interfaces;

namespace Vertr.TinvestGateway.Application.Proxy;

internal class TinvestGatewayAccounts : TinvestGatewayBase, ITinvestGatewayAccounts
{
    public TinvestGatewayAccounts(InvestApiClient investApiClient) : base(investApiClient)
    {
    }

    public async Task<Account[]?> GetAccounts()
    {
        var response = await InvestApiClient.Users.GetAccountsAsync();
        var accounts = response.Accounts.ToArray().ToAccounts();

        return accounts;
    }
    public async Task<Account[]?> GetSandboxAccounts()
    {
        var response = await InvestApiClient.Sandbox.GetSandboxAccountsAsync(new Tinkoff.InvestApi.V1.GetAccountsRequest());
        var accounts = response.Accounts.ToArray().ToAccounts();

        return accounts;
    }
    public async Task<string> CreateSandboxAccount(string accountName)
    {
        var tRequest = new Tinkoff.InvestApi.V1.OpenSandboxAccountRequest
        {
            Name = accountName,
        };

        var response = await InvestApiClient.Sandbox.OpenSandboxAccountAsync(tRequest);

        return response.AccountId;
    }

    public async Task CloseSandboxAccount(string accountId)
    {
        var tRequest = new Tinkoff.InvestApi.V1.CloseSandboxAccountRequest
        {
            AccountId = accountId,
        };

        _ = await InvestApiClient.Sandbox.CloseSandboxAccountAsync(tRequest);
    }

    public async Task<Money?> PayIn(string accountId, Money money)
    {
        var tRequest = new Tinkoff.InvestApi.V1.SandboxPayInRequest
        {
            AccountId = accountId,
            Amount = money.Convert()
        };

        var response = await InvestApiClient.Sandbox.SandboxPayInAsync(tRequest);
        var balance = response.Balance.Convert();

        return balance;
    }

    public async Task<Operation[]?> GetOperations(string accountId, DateTime? from = null, DateTime? to = null)
    {
        var request = new Tinkoff.InvestApi.V1.OperationsRequest
        {
            AccountId = accountId,
        };

        if (from.HasValue)
        {
            request.From = Timestamp.FromDateTime(from.Value);
        }

        if (to.HasValue)
        {
            request.To = Timestamp.FromDateTime(to.Value);
        }

        var response = await InvestApiClient.Operations.GetOperationsAsync(request);
        var operations = response.Operations.ToArray().Convert();

        return operations;
    }


    public async Task<PortfolioSnapshot?> GetPortfolio(string accountId)
    {
        var request = new Tinkoff.InvestApi.V1.PortfolioRequest
        {
            AccountId = accountId,
            Currency = Tinkoff.InvestApi.V1.PortfolioRequest.Types.CurrencyRequest.Rub
        };

        var response = await InvestApiClient.Operations.GetPortfolioAsync(request);
        var portfolio = response.Convert();

        return portfolio;
    }

    public async Task<PositionsResponse?> GetPositions(string accountId)
    {
        var request = new Tinkoff.InvestApi.V1.PositionsRequest
        {
            AccountId = accountId,
        };

        var response = await InvestApiClient.Operations.GetPositionsAsync(request);
        var result = response.Convert(accountId);

        return result;
    }
}
