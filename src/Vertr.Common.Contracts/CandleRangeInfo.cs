namespace Vertr.Common.Contracts;

public readonly record struct CandleRangeInfo
{
    public DateTime From { get; init; }

    public DateTime To { get; init; }

    public int Count { get; init; }
}

