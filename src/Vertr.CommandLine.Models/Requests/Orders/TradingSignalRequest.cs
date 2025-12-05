using Vertr.CommandLine.Common.Mediator;

namespace Vertr.CommandLine.Models.Requests.Orders;

public class TradingSignalRequest : IRequest<TradingSignalResponse>
{
    public Guid PortfolioId { get; init; }

    public required string Symbol { get; init; }

    public Direction Direction { get; init; }

    public DateTime MarketTime { get; init; }

    public decimal OpenPositionQty { get; init; }

    public decimal ComissionPercent { get; init; }
}

public class TradingSignalResponse : ResponseBase
{
    public Trade[] Trades { get; init; } = [];
}

public enum Direction
{
    Hold = 0,
    Buy = 1,
    Sell = -1,
}