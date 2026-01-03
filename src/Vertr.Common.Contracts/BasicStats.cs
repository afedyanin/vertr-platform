namespace Vertr.Common.Contracts;

public record struct BasicStats
{
    public double Mean { get; set; }

    public double StdDev { get; set; }

    public int Count { get; set; }

    public override string ToString()
    {
        return $"{Mean:N2} +/- {StdDev:N2} (n={Count})";
    }
}
