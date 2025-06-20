using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.Converters;

internal static class PortfolioSnapshotConverter
{
    public static PortfolioSnapshot Convert(
        this PortfolioSnapshot source)
        => new PortfolioSnapshot(
            source.AccountId,
            source.BookId,
            source.UpdatedAt,
            source.Positions.ToArray().Convert());

    public static PortfolioSnapshot[] Convert(
        this PortfolioSnapshot[] source)
        => [.. source.Select(Convert)];

    public static PortfolioPosition Convert(
        this PortfolioPosition source)
        => new PortfolioPosition(
            source.InstrumentId,
            source.Balance);

    public static PortfolioPosition[] Convert(
        this PortfolioPosition[] source)
        => [.. source.Select(Convert)];
}

