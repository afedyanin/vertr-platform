using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

public interface IPortfolioManager
{
    public MarketOrderRequest? HandleTradingSignal(TradingSignal signal, bool reverseOnly = false);
    public Task<IEnumerable<MarketOrderRequest>> CloseAllPositions();
}
