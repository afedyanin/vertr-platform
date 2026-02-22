using Refit;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

public interface ITradingGateway
{
    public Task<string?> PostMarketOrder(MarketOrderRequest request);

    public Task<Instrument[]> GetAllInstruments();

    public Task<Candle[]> GetCandles(Guid instrumentId, [Query] int maxItems = -1);

    public Task<OrderTrades[]> FindOrderTrades(string orderId);

    public Task<Portfolio?> GetPortfolio(Guid portfolioId);
}
