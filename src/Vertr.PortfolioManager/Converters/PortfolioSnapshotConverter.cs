using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.Converters;

internal static class PortfolioSnapshotConverter
{
    public static PortfolioSnapshot Convert(
        this Application.Entities.PortfolioSnapshot source)
        => new PortfolioSnapshot(
            source.AccountId,
            source.BookId,
            source.UpdatedAt,
            source.Positions.ToArray().Convert());

    public static PortfolioSnapshot[] Convert(
        this Application.Entities.PortfolioSnapshot[] source)
        => [.. source.Select(Convert)];

    public static PortfolioPosition Convert(
        this Application.Entities.PortfolioPosition source)
        => new PortfolioPosition(
            source.InstrumentId,
            source.Balance);

    public static PortfolioPosition[] Convert(
        this Application.Entities.PortfolioPosition[] source)
        => [.. source.Select(Convert)];
}

