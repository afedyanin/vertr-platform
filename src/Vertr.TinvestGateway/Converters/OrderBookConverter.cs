using Vertr.Common.Contracts;

namespace Vertr.TinvestGateway.Converters;

public static class OrderBookConverter
{
    public static OrderBook Convert(this Tinkoff.InvestApi.V1.OrderBook orderBook)
        => new OrderBook
        {
            InstrumentId = Guid.Parse(orderBook.InstrumentUid),
            UpdatedAt = orderBook.Time.ToDateTime(),
            Depth = orderBook.Depth,
            IsConsistent = orderBook.IsConsistent,
            Bids = orderBook.Bids.ToArray().Convert(),
            Asks = orderBook.Asks.ToArray().Convert(),
        };

    public static Order[] Convert(this Tinkoff.InvestApi.V1.Order[] orders)
        => [.. orders.Select(Convert)];

    public static Order Convert(this Tinkoff.InvestApi.V1.Order order)
        => new Order
        {
            Price = order.Price,
            QtyLots = order.Quantity,
        };
}
