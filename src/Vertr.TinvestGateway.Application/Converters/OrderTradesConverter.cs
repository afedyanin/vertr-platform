using Vertr.OrderExecution.Contracts;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.TinvestGateway.Application.Converters;

internal static class OrderTradesConverter
{
    public static OrderTrades Convert(
        this Tinkoff.InvestApi.V1.OrderTrades source)
        => new OrderTrades
        {
            OrderId = source.OrderId,
            CreatedAt = source.CreatedAt.ToDateTime(),
            Direction = source.Direction.Convert(),
            InstrumentId = Guid.Parse(source.InstrumentUid),
            Trades = source.Trades.ToArray().Convert("")
        };

    public static Trade Convert(this Tinkoff.InvestApi.V1.OrderTrade source, string currency)
        => new Trade
        {
            ExecutionTime = source.DateTime.ToDateTime(),
            Price = new Money(source.Price, currency),
            Quantity = source.Quantity,
            TradeId = source.TradeId,
        };

    public static Trade[] Convert(
        this Tinkoff.InvestApi.V1.OrderTrade[] source, string currency)
        => [.. source.Select(t => t.Convert(currency))];

    public static Trade Convert(this Tinkoff.InvestApi.V1.OrderStage source)
        => new Trade
        {
            ExecutionTime = source.ExecutionTime.ToDateTime(),
            Price = source.Price.Convert(),
            Quantity = source.Quantity,
            TradeId = source.TradeId,
        };

    public static Trade[] Convert(this Tinkoff.InvestApi.V1.OrderStage[] source)
        => [.. source.Select(t => t.Convert())];

}
