namespace Vertr.Portfolio.Domain;

public record class MoneyValue
{
    public required string Currency { get; init; }

    public decimal Value { get; init; }
}
