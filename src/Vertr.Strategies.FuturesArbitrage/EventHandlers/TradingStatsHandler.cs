using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;
using Vertr.Strategies.FuturesArbitrage.Abstractions;
using Vertr.Strategies.FuturesArbitrage.Models;

namespace Vertr.Strategies.FuturesArbitrage.EventHandlers;

internal sealed class TradingStatsHandler : IEventHandler<OrderBookChangedEvent>
{
    private readonly ITradingStatsLocalStorage _marketDataEventLocalStorage;

    public int HandlingOrder => 1010;

    public TradingStatsHandler(ITradingStatsLocalStorage marketDataEventLocalStorage)
    {
        _marketDataEventLocalStorage = marketDataEventLocalStorage;
    }

    public ValueTask OnEvent(OrderBookChangedEvent data)
    {
        if (data.OrderRequests.Any())
        {
            var tradingStats = ComposeTradingStats(data);
            _marketDataEventLocalStorage.Save(tradingStats);
        }

        return ValueTask.CompletedTask;
    }

    private IEnumerable<TradingStatsInfo> ComposeTradingStats(OrderBookChangedEvent data)
    {
        var tradingStats = new List<TradingStatsInfo>();

        if (!data.DerivedAssets.Any() || !data.OrderRequests.Any())
        {
            return tradingStats;
        }

        var baseAsset = ComposeBaseAssetInfo(data);

        foreach (var derivedAsset in data.DerivedAssets)
        {
            var orderRequest = data.OrderRequests.FirstOrDefault(r => r.PortfolioId == derivedAsset.PortfolioId);

            if (orderRequest == null)
            {
                continue;
            }

            var tradingStatsInfo = new TradingStatsInfo
            {
                BaseAsset = baseAsset,
                DerivedAsset = derivedAsset,
                OrderExecutionInfo = ComposeOrderExecutionInfo(orderRequest),
            };

            tradingStats.Add(tradingStatsInfo);
        }

        return tradingStats;
    }

    private OrderExecutionInfo ComposeOrderExecutionInfo(MarketOrderRequest orderRequest)
        => new OrderExecutionInfo
        {
            InstrumentId = orderRequest.InstrumentId,
            PortfolioId = orderRequest.PortfolioId,
            OrderRequestId = orderRequest.RequestId,
            OrderId = orderRequest.OrderId,
        };

    private BaseAssetInfo ComposeBaseAssetInfo(OrderBookChangedEvent data)
    {
        var ob = data.OrderBook;

        var baseAssetInfo = new BaseAssetInfo
        {
            InstrumentId = ob.InstrumentId,
            UpdatedAt = ob.UpdatedAt,
            MaxBid = ob.MaxBid,
            MinAsk = ob.MinAsk,
            Direction = data.TradingDirection,
        };

        return baseAssetInfo;
    }
}

