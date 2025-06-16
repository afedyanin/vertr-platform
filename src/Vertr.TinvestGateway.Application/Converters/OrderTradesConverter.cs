using Vertr.TinvestGateway.Contracts;

namespace Vertr.TinvestGateway.Application.Converters;

internal static class OrderTradesConverter
{
    public static OrderTrades Convert(this Tinkoff.InvestApi.V1.OrderTrades source)
        => new OrderTrades
        {
            OrderId = source.OrderId,
            AccountId = source.AccountId,
            CreatedAt = source.CreatedAt.ToDateTime(),
            Direction = source.Direction.Convert(),
            InstrumentId = source.InstrumentUid,
            Trades = source.Trades.ToArray().Convert()
        };
}
