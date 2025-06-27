using Vertr.OrderExecution.Contracts;
using Vertr.TinvestGateway.Contracts;

namespace Vertr.PortfolioManager.Contracts;

public record class Operation
{
    public string? ParentOperationId { get; init; }

    public string Currency { get; init; } = string.Empty;

    public decimal Payment { get; init; }

    public decimal Price { get; init; }

    public OperationState State { get; init; }

    public long Quantity { get; init; }

    public long QuantityRest { get; init; }

    public string InstrumentType { get; init; } = string.Empty;

    public DateTime Date { get; init; }

    public string Type { get; init; } = string.Empty;

    public OperationType OperationType { get; init; }

    public string? AssetUid { get; init; }

    public string? PositionUid { get; init; }

    public string? InstrumentId { get; init; }

    public Trade[] OperationTrades { get; init; } = [];
}
