namespace Vertr.OrderExecution.Application.Abstractions;

// TODO: Move it to market data service
public interface IStaticMarketDataProvider
{
    public long PositionToLots(Guid instrumentId, decimal position);
}
