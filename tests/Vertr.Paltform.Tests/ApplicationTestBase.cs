using Refit;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.Platform.Host;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.Paltform.Tests;

public abstract class ApplicationTestBase
{
    private const decimal _initialAmount = 100_000;
    private static readonly Guid _sber = new Guid("e6123145-9665-43e0-8413-cd61b8aa9b13");

    private const string _baseAddress = "https://localhost:7085";

    protected IVertrPlatformClient VertrClient { get; private set; }

    public ApplicationTestBase()
    {
        VertrClient = RestService.For<IVertrPlatformClient>(_baseAddress);
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

    protected Portfolio? GetPortfolio(string accountId)
    {
        var res = VertrClient.GetPortfolio(accountId);
        return res;
    }

    protected async Task<ExecuteOrderResponse?> OpenPosition(PortfolioIdentity portfolioIdentity, long qtyLots)
    {
        var req = new OpenPositionRequest
        {
            RequestId = Guid.NewGuid(),
            PortfolioIdentity = portfolioIdentity,
            InstrumentId = _sber,
            QtyLots = qtyLots,
        };

        var res = await VertrClient.OpenPosition(req);

        return res;
    }

    protected async Task<ExecuteOrderResponse?> ReversePosition(PortfolioIdentity portfolioIdentity)
    {
        var req = new ReversePositionRequest
        {
            RequestId = Guid.NewGuid(),
            PortfolioIdentity = portfolioIdentity,
            InstrumentId = _sber,
        };

        var res = await VertrClient.RevertPosition(req);

        return res;
    }
}
