using Vertr.Common.Contracts;

namespace Vertr.Strategies.FuturesArbitrage.Models;

public class ClosePositionStatsCsvItem
{
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

    public static IEnumerable<ClosePositionStatsCsvItem> Create(IEnumerable<OrderTrades> orderTrades)
    {
        var res = new List<ClosePositionStatsCsvItem>();

        foreach (var item in orderTrades)
        {
            var items = Create(item);
            res.AddRange(items);
        }

        return res;
    }

    private static IEnumerable<ClosePositionStatsCsvItem> Create(OrderTrades orderTrades)
    {
        var res = new List<ClosePositionStatsCsvItem>();

        foreach (var trade in orderTrades.Trades)
        {
            var item = Create(orderTrades, trade);
            res.Add(item);
        }

        return res;
    }

    private static ClosePositionStatsCsvItem Create(OrderTrades orderTrades, Trade trade)
    {
        var res = new ClosePositionStatsCsvItem
        {
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
