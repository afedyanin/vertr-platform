namespace Vertr.Common.Contracts;

public record class PortfolioInstrumentStats
{
    public Guid InstrumentId { get; init; }

    public BasicStats PositionsStats { get; init; }

    public BasicStats CommissionsStats { get; init; }

    public override string? ToString()
    {
        return $"Pos=[{PositionsStats.Mean} +-{PositionsStats.StdDev} N={PositionsStats.Count}] Com=[{CommissionsStats.Mean} +-{CommissionsStats.StdDev} N={CommissionsStats.Count}]";
    }
}
