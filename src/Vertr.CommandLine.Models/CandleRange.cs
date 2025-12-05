namespace Vertr.CommandLine.Models;

public record class CandleRange
{
    public required string Symbol { get; init; }

    public DateTime FirstDate { get; init; }

    public DateTime LastDate { get; init; }

    public int Count { get; init; }
}