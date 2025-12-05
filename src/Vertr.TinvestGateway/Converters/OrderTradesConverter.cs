using Vertr.TinvestGateway.Contracts.Orders;
using Vertr.TinvestGateway.Contracts.Portfolios;

namespace Vertr.TinvestGateway.Converters;

public static class OrderTradesConverter
{
    public static OrderTrades Convert(
        this Tinkoff.InvestApi.V1.OrderTrades source,
        string currency = "rub")
        => new OrderTrades
        {
            Id = Guid.NewGuid(),
            OrderId = source.OrderId,
            CreatedAt = source.CreatedAt.ToDateTime(),
            Direction = source.Direction.Convert(),
            InstrumentId = Guid.Parse(source.InstrumentUid),
            Trades = source.Trades.ToArray().Convert(currency)
        };

    public static Trade Convert(
        this Tinkoff.InvestApi.V1.OrderTrade source, string currency = "rub")
        => new Trade
        {
            ExecutionTime = source.DateTime.ToDateTime(),
            Price = new Money(source.Price, currency),
            Quantity = source.Quantity,
            TradeId = source.TradeId,
        };

    public static Trade[] Convert(
        this Tinkoff.InvestApi.V1.OrderTrade[] source, string currency = "rub")
        => [.. source.Select(t => t.Convert(currency))];

    public static Trade Convert(this Tinkoff.InvestApi.V1.OrderStage source)
        => new Trade
        {
            ExecutionTime = source.ExecutionTime.ToDateTime(),
            Price = source.Price.Convert(),
            Quantity = source.Quantity,
            TradeId = source.TradeId,
        };
}