namespace Vertr.Common.Contracts;

public record class PortfolioInstrumentStats
{
    public Guid InstrumentId { get; init; }

    public BasicStats PositionsStats { get; init; }

    public BasicStats CommissionsStats { get; init; }
}
