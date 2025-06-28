namespace Vertr.PortfolioManager.Contracts;
public record class TradeOperation
{
    public Guid Id { get; init; }

    public DateTime CreatedAt { get; init; }

    public TradeOperationType OperationType { get; init; }

    public string? OrderId { get; init; }

    public required string AccountId { get; init; }

    public Guid? BookId { get; init; }

    public Guid InstrumentId { get; init; }

    public decimal? Amount { get; init; }

    public string? Message { get; init; }

    public string? TradeId { get; init; }

    public DateTime ExecutionTime { get; init; }

    public decimal Price { get; init; }

    public long Quantity { get; init; }
}
