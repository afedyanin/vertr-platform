namespace Vertr.Contracts;

public record class MoneyValue
{
    public required string Currency { get; init; }

    public decimal Value { get; init; }
}
