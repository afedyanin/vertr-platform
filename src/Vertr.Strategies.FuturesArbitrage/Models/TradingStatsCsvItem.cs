using Vertr.Common.Contracts;

namespace Vertr.Strategies.FuturesArbitrage.Models;

public class TradingStatsCsvItem
{
    public int Sequence { get; set; }
    public Guid BaseAsset_InfoInstrumentId { get; set; }
    public DateTime BaseAsset_UpdatedAt { get; set; }
    public decimal BaseAsset_MaxBid { get; set; }
    public decimal BaseAsset_MinAsk { get; set; }
    public TradingDirection BaseAsset_Direction { get; set; }

    public Guid DerivedAsset_InstrumentId { get; set; }
    public Guid? DerivedAsset_PortfolioId { get; set; }
    public DateTime DerivedAsset_UpdatedAt { get; set; }
    public decimal DerivedAsset_FairPrice { get; set; }
    public decimal DerivedAsset_MaxBid { get; set; }
    public decimal DerivedAsset_MinAsk { get; set; }
    public double DerivedAsset_Threshold { get; set; }
    public TradingDirection DerivedAsset_Direction { get; set; }

    public Guid OrderExecution_InstrumentId { get; set; }
    public Guid OrderExecution_PortfolioId { get; set; }
    public Guid OrderExecution_OrderRequestId { get; set; }
    public string? OrderExecution_OrderId { get; set; }

    public required Guid OrderTrades_Id { get; set; }
    public string OrderTrades_OrderId { get; set; } = string.Empty;
    public DateTime OrderTrades_CreatedAt { get; set; }
    public OrderDirection OrderTrades_Direction { get; set; }
    public Guid OrderTrades_InstrumentId { get; set; }

    public string Trade_TradeId { get; set; } = string.Empty;
    public DateTime Trade_ExecutionTime { get; set; }
    public decimal Trade_Price { get; set; }
    public required string Trade_Currency { get; set; }
    public long Trade_Quantity { get; set; }


    public static IEnumerable<TradingStatsCsvItem> Create(IEnumerable<TradingStatsInfo> statsInfos)
    {
        var res = new List<TradingStatsCsvItem>();

        foreach (var info in statsInfos)
        {
            var items = Create(info);
            res.AddRange(items);
        }

        return res;
    }

    private static IEnumerable<TradingStatsCsvItem> Create(TradingStatsInfo statsInfo)
    {
        var res = new List<TradingStatsCsvItem>();

        foreach (var orderTrade in statsInfo.OrderTrades)
        {
            var items = Create(
                statsInfo.Sequence,
                statsInfo.BaseAsset,
                statsInfo.DerivedAsset,
                statsInfo.OrderExecutionInfo,
                orderTrade);

            res.AddRange(items);
        }

        return res;
    }

    private static IEnumerable<TradingStatsCsvItem> Create(
        int sequence,
        BaseAssetInfo baseAsset,
        DerivedAssetInfo derivedAsset,
        OrderExecutionInfo orderExecutionInfo,
        OrderTrades orderTrades)
    {
        var res = new List<TradingStatsCsvItem>();

        foreach (var trade in orderTrades.Trades)
        {
            var item = Create(sequence, baseAsset, derivedAsset, orderExecutionInfo, orderTrades, trade);
            res.Add(item);
        }

        return res;
    }

    private static TradingStatsCsvItem Create(
        int sequence,
        BaseAssetInfo baseAsset,
        DerivedAssetInfo derivedAsset,
        OrderExecutionInfo orderExecutionInfo,
        OrderTrades orderTrades,
        Trade trade)
    {
        var res = new TradingStatsCsvItem
        {
            Sequence = sequence,
            BaseAsset_InfoInstrumentId = baseAsset.InstrumentId,
            BaseAsset_UpdatedAt = baseAsset.UpdatedAt,
            BaseAsset_MaxBid = baseAsset.MaxBid,
            BaseAsset_MinAsk = baseAsset.MinAsk,
            BaseAsset_Direction = baseAsset.Direction,
            DerivedAsset_InstrumentId = derivedAsset.InstrumentId,
            DerivedAsset_PortfolioId = derivedAsset.PortfolioId,
            DerivedAsset_UpdatedAt = derivedAsset.UpdatedAt,
            DerivedAsset_FairPrice = derivedAsset.FairPrice,
            DerivedAsset_MaxBid = derivedAsset.MaxBid,
            DerivedAsset_MinAsk = derivedAsset.MinAsk,
            DerivedAsset_Threshold = derivedAsset.Threshold,
            DerivedAsset_Direction = derivedAsset.Direction,
            OrderExecution_InstrumentId = orderExecutionInfo.InstrumentId,
            OrderExecution_PortfolioId = orderExecutionInfo.PortfolioId,
            OrderExecution_OrderRequestId = orderExecutionInfo.OrderRequestId,
            OrderExecution_OrderId = orderExecutionInfo.OrderId,
            OrderTrades_Id = orderTrades.Id,
            OrderTrades_OrderId = orderTrades.OrderId,
            OrderTrades_CreatedAt = orderTrades.CreatedAt,
            OrderTrades_Direction = orderTrades.Direction,
            OrderTrades_InstrumentId = orderTrades.InstrumentId,
            Trade_TradeId = trade.TradeId,
            Trade_ExecutionTime = trade.ExecutionTime,
            Trade_Price = trade.Price,
            Trade_Currency = trade.Currency,
            Trade_Quantity = trade.Quantity,
        };

        return res;
    }
}
