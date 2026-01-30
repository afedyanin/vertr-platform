using Refit;
using Vertr.Common.Contracts;

namespace Vertr.Clients.TinvestApiClient;

public interface ITinvestGatewayClient
{
    [Post("/api/tinvest/orders/market")]
    public Task PostMarketOrder(MarketOrderRequest request);

    [Get("/api/instruments/all")]
    public Task<Instrument[]> GetAllInstruments();

    [Get("/api/candles/{instrumentId}")]
    public Task<Candle[]> GetCandles(Guid instrumentId, [Query] int maxItems = -1);
}
