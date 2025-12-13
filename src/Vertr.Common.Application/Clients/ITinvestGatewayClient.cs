using Refit;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Clients;

public interface ITinvestGatewayClient
{
    [Post("/api/tinvest/orders/market")]
    public Task PostMarketOrder(MarketOrderRequest request);

    [Get("/api/instruments/all")]
    public Task<Instrument[]> GetAllInstruments();

    [Get("api/candles/{instrumentId}")]
    public Task<Candle[]> GetCandles(Guid instrumentId, [Query] long maxItems = -1);
}