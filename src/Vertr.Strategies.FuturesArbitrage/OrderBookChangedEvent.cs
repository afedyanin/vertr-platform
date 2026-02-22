using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;
using Vertr.Strategies.FuturesArbitrage.Models;

namespace Vertr.Strategies.FuturesArbitrage;

public record class OrderBookChangedEvent : IMarketDataEvent
{
    public int Sequence { get; init; }

    public required OrderBook OrderBook { get; init; }

    public TradingDirection TradingDirection { get; set; } = TradingDirection.Hold;

    public Dictionary<Guid, decimal?> FairPrices { get; set; } = [];

    public List<DerivedAssetInfo> DerivedAssets { get; set; } = [];

    public List<TradingSignal> TradingSignals { get; } = [];

    public List<MarketOrderRequest> OrderRequests { get; } = [];
}
