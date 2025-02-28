using Vertr.Domain.Enums;

namespace Vertr.Domain;

public record class Operation
{
    public Guid Id { get; set; }

    public Guid? ParentOperationId { get; set; }

    public string AccountId { get; set; }

    public string Currency { get; set; }

    public decimal Payment { get; set; }

    public decimal Price { get; set; }

    public OperationState State { get; set; }

    public long Quantity { get; set; }

    public long QuantityRest { get; set; }

    public string InstrumentType { get; set; }

    public DateTime Date { get; set; }

    public string Type { get; set; }

    public OperationType OperationType { get; set; }

    public Guid? AssetUid { get; set; }

    public Guid PositionUid { get; set; }

    public Guid? InstrumentUid { get; set; }

    public IEnumerable<OperationTrade> OperationTrades { get; set; } = [];
}
