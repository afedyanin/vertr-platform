namespace Vertr.Common.Contracts;

public record class PortfolioInstrumentStats
{
    public Guid InstrumentId { get; init; }

    public BasicStats PositionsStats { get; init; }

    public BasicStats CommissionsStats { get; init; }

    public override string ToString()
    {
        return $"Pos=[{PositionsStats}] Com=[{CommissionsStats}]";
    }

    public IEnumerable<(string, double)> ToColumns(string prefix)
    {
        var cols = new (string, double)[]
        {
            ($"{prefix}_pos_mean", PositionsStats.Mean),
            ($"{prefix}_pos_std", PositionsStats.StdDev),
            ($"{prefix}_pos_cnt", PositionsStats.Count),
            ($"{prefix}_com_mean", CommissionsStats.Mean),
            ($"{prefix}_com_std", CommissionsStats.StdDev),
            ($"{prefix}_com_cnt", CommissionsStats.Count)
        };

        return cols;
    }
}
