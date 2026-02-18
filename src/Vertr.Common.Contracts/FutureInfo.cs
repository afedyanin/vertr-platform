namespace Vertr.Common.Contracts;

public record class FutureInfo
{
    public required string Ticker { get; set; }

    public required string Name { get; set; }

    public DateOnly ExpDate { get; set; }

    public int LotSize { get; set; }

    public required string Unit { get; set; }
}
