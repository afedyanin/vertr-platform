using Vertr.OrderExecution.Application.Abstractions;

namespace Vertr.OrderExecution.Application.Services;

// TODO: Move it to market data service
public class StaticMarketDataProvider : IStaticMarketDataProvider
{
    public long PositionToLots(Guid instrumentId, decimal position)
        => (long)(position / 10.0m);
}
