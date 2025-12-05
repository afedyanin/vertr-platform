using Tinkoff.InvestApi;

namespace Vertr.TinvestGateway.Proxy;

internal abstract class TinvestGatewayBase
{
    protected InvestApiClient InvestApiClient { get; private set; }

    protected TinvestGatewayBase(InvestApiClient investApiClient)
    {
        InvestApiClient = investApiClient;
    }
}