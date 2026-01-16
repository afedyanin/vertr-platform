using Vertr.Common.Contracts;

namespace Vertr.TinvestGateway.Converters;

public static class MarketTradeConverter
{
    public static MarketTrade Convert(this Tinkoff.InvestApi.V1.Trade trade)
        => new MarketTrade
        {
            Time = trade.Time.ToDateTime(),
            InstrumentId = Guid.Parse(trade.InstrumentUid),
            Quantity = trade.Quantity,
            Price = trade.Price,
            Direction = trade.Direction == Tinkoff.InvestApi.V1.TradeDirection.Buy ? TradingDirection.Buy :
                trade.Direction == Tinkoff.InvestApi.V1.TradeDirection.Sell ? TradingDirection.Sell : TradingDirection.Hold,
        };
}
