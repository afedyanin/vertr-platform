namespace Vertr.OrderExecution.Contracts;
public record class Trade
{
    public required string Id { get; init; }

    public decimal Quantity { get; init; }

    public decimal Price { get; init; }

    public DateTime ExecutionTime { get; init; }
}
