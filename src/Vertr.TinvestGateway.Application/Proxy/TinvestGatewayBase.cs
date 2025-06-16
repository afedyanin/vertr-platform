using Tinkoff.InvestApi;

namespace Vertr.TinvestGateway.Application.Proxy;

internal abstract class TinvestGatewayBase
{
    protected InvestApiClient InvestApiClient { get; private set; }

    protected TinvestGatewayBase(InvestApiClient investApiClient)
    {
        InvestApiClient = investApiClient;
    }
}
