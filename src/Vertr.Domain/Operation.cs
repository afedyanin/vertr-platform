using Vertr.Domain.Enums;

namespace Vertr.Domain;

public record class Operation
{
    public string Id { get; set; }

    public string ParentOperationId { get; set; }

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

    public string AssetUid { get; set; }

    public string PositionUid { get; set; }

    public string InstrumentUid { get; set; }

    public IEnumerable<OperationTrade> OperationTrades { get; set; } = [];
}
