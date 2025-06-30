using Refit;
using Tinkoff.InvestApi.V1;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.Platform.Host;
using Vertr.Platform.Host.Requests;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.Platform.Tests;

public abstract class ApplicationTestBase
{
    private const string _baseAddress = "https://localhost:7085";

    private static readonly Guid _sber = new Guid("e6123145-9665-43e0-8413-cd61b8aa9b13");

    private static readonly string _accountId = "93cda594-5556-44ca-8005-1c893e8d3142";
    private static readonly Guid _subAccountId = new Guid("93cda594-5555-44ca-8005-1c893e8d3142");

    protected IVertrPlatformClient VertrClient { get; private set; }

    public ApplicationTestBase()
    {
        VertrClient = RestService.For<IVertrPlatformClient>(_baseAddress);
    }

    protected async Task<Portfolio?> GetPortfolio()
    {
        var res = await VertrClient.GetPortfolio(_accountId, _subAccountId);
        return res;
    }

    protected async Task<ExecuteOrderResponse?> OpenPosition(long qtyLots)
    {
        var req = new OpenRequest(_sber, _accountId, _subAccountId, qtyLots);
        var res = await VertrClient.OpenPosition(req);

        return res;
    }

    protected async Task<ExecuteOrderResponse?> ClosePosition()
    {
        var req = new CloseRequest(_sber, _accountId, _subAccountId);
        var res = await VertrClient.ClosePosition(req);

        return res;
    }

    protected async Task<ExecuteOrderResponse?> ReversePosition()
    {
        var req = new ReverseRequest(_sber, _accountId, _subAccountId);
        var res = await VertrClient.RevertPosition(req);

        return res;
    }

    protected async Task PayIn(decimal amount)
    {
        _ = await VertrClient.PayIn(_accountId, _subAccountId, amount);
    }

    /*
    protected async Task<string> OpenAccount(decimal amount = _initialAmount)
    {
        var accountId = await VertrClient.CreateAccount("application test");
        _ = await VertrClient.PayIn(accountId, new Money(amount, "RUB"));
        return accountId;
    }

    protected async Task CloseAccount(string accountId)
    {
        await VertrClient.CloseAccount(accountId);
    }
      
    protected async Task<TradeOperation[]?> GetGatewayOperations(string accountId)
    {
        var ops = await VertrClient.GetGatewayOperations(accountId);
        return ops;
    }

    protected async Task<Portfolio?> GetGatewayPortfolio(string accountId)
    {
        var portfolio = await VertrClient.GetGatewayPortfolio(accountId);
        return portfolio;
    }
    */
}
