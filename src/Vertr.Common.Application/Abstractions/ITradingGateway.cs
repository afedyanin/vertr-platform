using Refit;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

internal interface ITradingGateway
{
    public Task PostMarketOrder(MarketOrderRequest request);

    public Task<Instrument[]> GetAllInstruments();

    public Task<Candle[]> GetCandles(Guid instrumentId, [Query] long maxItems = -1);
}
