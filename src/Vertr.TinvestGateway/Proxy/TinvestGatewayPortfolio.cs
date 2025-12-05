using Google.Protobuf.WellKnownTypes;
using Tinkoff.InvestApi;
using Vertr.TinvestGateway.Abstractions;
using Vertr.TinvestGateway.Contracts.Portfolios;
using Vertr.TinvestGateway.Converters;

namespace Vertr.TinvestGateway.Proxy;

internal class TinvestGatewayPortfolio : TinvestGatewayBase, IPortfolioGateway
{
    public TinvestGatewayPortfolio(InvestApiClient investApiClient) : base(investApiClient)
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

    public async Task<Portfolio?> GetPortfolio(string accountId)
    {
        var request = new Tinkoff.InvestApi.V1.PortfolioRequest
        {
            AccountId = accountId,
            Currency = Tinkoff.InvestApi.V1.PortfolioRequest.Types.CurrencyRequest.Rub
        };

        var response = await InvestApiClient.Operations.GetPortfolioAsync(request);
        var portfolio = response.Convert(DateTime.UtcNow);

        return portfolio;
    }

    public async Task<TradeOperation[]?> GetOperations(string accountId, DateTime from, DateTime to)
    {
        var request = new Tinkoff.InvestApi.V1.GetOperationsByCursorRequest
        {
            AccountId = accountId,
            From = Timestamp.FromDateTime(from.ToUniversalTime()),
            To = Timestamp.FromDateTime(to.ToUniversalTime()),
            Limit = 1000,
        };

        var operations = new List<TradeOperation>();

        var response = await InvestApiClient.Operations.GetOperationsByCursorAsync(request);
        var converted = response.Convert(accountId);
        operations.AddRange(converted);

        while (response != null && response.HasNext)
        {
            request = new Tinkoff.InvestApi.V1.GetOperationsByCursorRequest
            {
                AccountId = accountId,
                From = Timestamp.FromDateTime(from.ToUniversalTime()),
                To = Timestamp.FromDateTime(to.ToUniversalTime()),
                Limit = 1000,
                Cursor = response.NextCursor
            };

            response = await InvestApiClient.Operations.GetOperationsByCursorAsync(request);
            converted = response.Convert(accountId);
            operations.AddRange(converted);
        }

        return [.. operations];
    }
}