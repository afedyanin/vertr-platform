using Vertr.Common.Application.Abstractions;

namespace Vertr.Strategies.FuturesArbitrage;

public record class OrderBookChangedEvent : IMarketDataEvent
{
    public int Sequence { get; init; }
}
