using Refit;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Gateways;

internal sealed class BacktestGateway : ITradingGateway
{
    public Task<Instrument[]> GetAllInstruments()
    {
        throw new NotImplementedException();
    }

    public Task<Candle[]> GetCandles(Guid instrumentId, [Query] long maxItems = -1)
    {
        throw new NotImplementedException();
    }

    public Task PostMarketOrder(MarketOrderRequest request)
    {
        throw new NotImplementedException();
    }
}
