using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

public interface IPortfolioManager
{
    public MarketOrderRequest? HandleTradingSignal(TradingSignal signal);
    public Task CloseAllPositions();
}
