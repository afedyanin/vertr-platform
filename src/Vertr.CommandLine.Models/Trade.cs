namespace Vertr.CommandLine.Models;

public record class Trade
{
    public decimal Price { get; init; }

    public decimal Quantity { get; init; }

    public decimal Comission { get; init; }
}