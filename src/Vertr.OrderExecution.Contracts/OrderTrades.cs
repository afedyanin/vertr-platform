using Vertr.OrderExecution.Contracts.Enums;

namespace Vertr.OrderExecution.Contracts;

public record class OrderTrades
{
    public string OrderId { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public OrderDirection Direction { get; init; }

    public string InstrumentId { get; init; } = string.Empty;

    public string AccountId { get; init; } = string.Empty;

    public Trade[] Trades { get; init; } = [];
}
