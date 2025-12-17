namespace Vertr.Common.Contracts;

public record class Trade
{
    public string TradeId { get; init; } = string.Empty;

    public DateTime ExecutionTime { get; init; }

    public decimal Price { get; init; }

    public required string Currency { get; init; }

    public long Quantity { get; init; }
}