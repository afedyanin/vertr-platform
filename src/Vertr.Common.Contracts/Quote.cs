namespace Vertr.Common.Contracts;

public record struct Quote
{
    public decimal Bid { get; set; }

    public decimal Ask { get; set; }
}
