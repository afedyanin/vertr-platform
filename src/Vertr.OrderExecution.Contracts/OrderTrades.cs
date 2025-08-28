using Vertr.OrderExecution.Contracts.Enums;

namespace Vertr.OrderExecution.Contracts;

public record class OrderTrades
{
    public string OrderId { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public OrderDirection Direction { get; init; }

    public Guid InstrumentId { get; init; }

    public Trade[] Trades { get; init; } = [];

    public Guid? PortfolioId { get; init; }
}
