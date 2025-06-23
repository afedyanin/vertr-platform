using Refit;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.Platform.Host;
using Vertr.PortfolioManager.Contracts;
using Vertr.TinvestGateway.Contracts;

namespace Vertr.Paltform.Tests;

public abstract class ApplicationTestBase
{
    private const decimal _initialAmount = 100_000;
    private static readonly Guid _sberId = new Guid("e6123145-9665-43e0-8413-cd61b8aa9b13");

    protected IVertrPlatformClient VertrClient { get; private set; }

    public ApplicationTestBase(string baseAddress)
    {
        VertrClient = RestService.For<IVertrPlatformClient>(baseAddress);
    }

    protected async Task<string> OpenAccount(decimal amount = _initialAmount)
    {
        var accountId = await VertrClient.CreateAccount("application test");
        _ = await VertrClient.PayIn(accountId, new Money(amount));
        return accountId;
    }

    protected async Task CloseAccount(string accountId)
    {
        await VertrClient.CloseAccount(accountId);
    }

    protected async Task<PortfolioSnapshot?> GetPortfolio(string accountId)
    {
        var res = await VertrClient.MakeSnapshot(accountId);
        return res;
    }

    protected async Task<ExecuteOrderResponse?> OpenPosition(PortfolioIdentity portfolioId, long qtyLots)
    {
        var req = new OpenPositionRequest
        {
            RequestId = Guid.NewGuid(),
            PortfolioId = portfolioId,
            QtyLots = qtyLots,
            InstrumentId = _sberId,
        };

        var res = await VertrClient.OpenPosition(req);

        return res;
    }

    protected async Task<ExecuteOrderResponse?> ReversePosition(PortfolioIdentity portfolioId)
    {
        var req = new ReversePositionRequest
        {
            RequestId = Guid.NewGuid(),
            PortfolioId = portfolioId,
            InstrumentId = _sberId,
        };

        var res = await VertrClient.RevertPosition(req);

        return res;
    }
}
