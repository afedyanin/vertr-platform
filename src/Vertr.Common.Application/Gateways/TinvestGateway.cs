using Refit;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Gateways;

internal sealed class TinvestGateway : ITradingGateway
{
    private readonly ITinvestGatewayClient _gatewayClient;

    public TinvestGateway(ITinvestGatewayClient gatewayClient)
    {
        _gatewayClient = gatewayClient;
    }

    public Task<Instrument[]> GetAllInstruments()
        => _gatewayClient.GetAllInstruments();

    public Task<Candle[]> GetCandles(Guid instrumentId, [Query] long maxItems = -1)
        => _gatewayClient.GetCandles(instrumentId, maxItems);

    public Task PostMarketOrder(MarketOrderRequest request)
        => _gatewayClient.PostMarketOrder(request);
}

public interface ITinvestGatewayClient
{
    [Post("/api/tinvest/orders/market")]
    public Task PostMarketOrder(MarketOrderRequest request);

    [Get("/api/instruments/all")]
    public Task<Instrument[]> GetAllInstruments();

    [Get("api/candles/{instrumentId}")]
    public Task<Candle[]> GetCandles(Guid instrumentId, [Query] long maxItems = -1);
}



