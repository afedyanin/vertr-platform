namespace Vertr.Common.Contracts;

public record struct Order
{
    public decimal Price { get; set; }

    public long QtyLots { get; set; }
}
