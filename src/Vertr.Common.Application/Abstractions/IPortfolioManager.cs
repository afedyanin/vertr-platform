using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

public interface IPortfolioManager
{
    public Task<MarketOrderRequest?> HandleTradingSignal(TradingSignal signal);
    public Task CloseAllPositions();
}
