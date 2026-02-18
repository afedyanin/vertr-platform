namespace Vertr.Common.Contracts;

public record class IndexRate
{
    public required string Ticker { get; set; }

    public DateTime Time { get; set; }

    public decimal Value { get; set; }
}
