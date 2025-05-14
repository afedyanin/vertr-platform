using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.Converters;

internal static class PortfolioSnapshotConverter
{
    public static PortfolioSnapshot Convert(
        this Application.Entities.PortfolioSnapshot source)
        => new PortfolioSnapshot(
            source.PortfolioId,
            source.AccountId,
            source.UpdatedAt,
            source.Positions.Convert());

    public static PortfolioPosition Convert(
        this Application.Entities.PortfolioPosition source)
        => new PortfolioPosition(
            source.InstrumentId,
            source.Balance);

    public static PortfolioPosition[] Convert(
        this Application.Entities.PortfolioPosition[] source)
        => source.Select(Convert).ToArray();
}

