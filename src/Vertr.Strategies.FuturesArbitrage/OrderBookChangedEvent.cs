using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Strategies.FuturesArbitrage;

public record class OrderBookChangedEvent : IMarketDataEvent
{
    public int Sequence { get; init; }

    public List<TradingSignal> TradingSignals { get; } = [];

    public List<MarketOrderRequest> OrderRequests { get; } = [];
}
