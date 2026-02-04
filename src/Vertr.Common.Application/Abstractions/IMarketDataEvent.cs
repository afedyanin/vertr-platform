using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

public interface IMarketDataEvent
{
    public int Sequence { get; init; }

    public List<TradingSignal> TradingSignals { get; }

    public List<MarketOrderRequest> OrderRequests { get; }
}
