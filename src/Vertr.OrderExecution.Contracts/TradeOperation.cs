using Vertr.MarketData.Contracts;
using Vertr.OrderExecution.Contracts.Enums;

namespace Vertr.OrderExecution.Contracts;
public record class TradeOperation
{
    public Guid Id { get; init; }

    public DateTime CreatedAt { get; init; }

    public TradeOperationType OperationType { get; init; }

    public string? OrderId { get; init; }

    public required PortfolioIdentity PortfolioIdentity { get; init; }

    public required InstrumentIdentity InstrumentIdentity { get; init; }

    public Trade[] Trades { get; init; } = [];

    public decimal? Amount { get; init; }

    public string? Message { get; init; }
}
