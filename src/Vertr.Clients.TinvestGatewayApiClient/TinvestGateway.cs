using Refit;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Clients.TinvestGatewayApiClient;

internal sealed class TinvestGateway : ITradingGateway
{
    private readonly ITinvestGatewayClient _gatewayClient;

    public TinvestGateway(ITinvestGatewayClient gatewayClient)
    {
        _gatewayClient = gatewayClient;
    }

    public Task<Instrument[]> GetAllInstruments()
        => _gatewayClient.GetAllInstruments();

    public Task<Candle[]> GetCandles(Guid instrumentId, [Query] int maxItems = -1)
        => _gatewayClient.GetCandles(instrumentId, maxItems);

    public Task PostMarketOrder(MarketOrderRequest request)
        => _gatewayClient.PostMarketOrder(request);
}
