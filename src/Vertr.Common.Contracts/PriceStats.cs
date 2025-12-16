namespace Vertr.Common.Contracts;

public record struct PriceStats
{
    public double Mean { get; set; }

    public double StdDev { get; set; }

    public int Count { get; set; }
}
