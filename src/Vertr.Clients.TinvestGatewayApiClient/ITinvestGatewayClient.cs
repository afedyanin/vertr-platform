using Refit;
using Vertr.Common.Contracts;

namespace Vertr.Clients.TinvestGatewayApiClient;

public interface ITinvestGatewayClient
{
    [Post("/api/tinvest/orders/market")]
    public Task<string?> PostMarketOrder(MarketOrderRequest request);

    [Get("/api/instruments/all")]
    public Task<Instrument[]> GetAllInstruments();

    [Get("/api/candles/{instrumentId}")]
    public Task<Candle[]> GetCandles(Guid instrumentId, [Query] int maxItems = -1);

    [Get("/api/order-storage/trades")]
    public Task<OrderTrades[]> FindOrderTrades([Query] string pattern);

    [Get("/api/portfolio/{portfolioId}")]
    public Task<Portfolio?> GetPortfolio(Guid portfolioId);
}
