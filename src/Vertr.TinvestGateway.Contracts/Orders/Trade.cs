using Vertr.TinvestGateway.Contracts.Portfolios;

namespace Vertr.TinvestGateway.Contracts.Orders;

public record class Trade
{
    public string TradeId { get; init; } = string.Empty;

    public DateTime ExecutionTime { get; init; }

    public Money? Price { get; init; }

    public long Quantity { get; init; }
}