namespace Vertr.TinvestGateway.Contracts;

public record class Operation
{
    public Guid Id { get; init; }

    public Guid? ParentOperationId { get; init; }

    public string AccountId { get; init; } = string.Empty;

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

    public Guid? AssetUid { get; init; }

    public Guid PositionUid { get; init; }

    public Guid? InstrumentUid { get; init; }

    public Trade[] OperationTrades { get; init; } = [];
}
