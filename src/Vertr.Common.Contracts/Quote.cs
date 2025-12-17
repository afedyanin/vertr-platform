namespace Vertr.Common.Contracts;

public record struct Quote
{
    public DateTime Time { get; set; }

    public decimal Bid { get; set; }

    public decimal Ask { get; set; }

    public decimal Mid => (Bid + Ask) / 2;
}
