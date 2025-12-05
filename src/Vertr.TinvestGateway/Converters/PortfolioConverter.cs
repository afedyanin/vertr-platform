using Vertr.TinvestGateway.Contracts.Portfolios;

namespace Vertr.TinvestGateway.Converters;

internal static class PortfolioConverter
{
    public static Portfolio? Convert(
        this Tinkoff.InvestApi.V1.PortfolioResponse source,
        DateTime updatedAt)
    {
        if (source == null)
        {
            return null;
        }

        var res = new Portfolio
        {
            Id = Guid.NewGuid(),
            UpdatedAt = updatedAt,
            Positions = source.Positions.ToArray().Convert()
        };

        return res;
    }

    private static Position Convert(
        this Tinkoff.InvestApi.V1.PortfolioPosition source)
        => new Position
        {
            InstrumentId = Guid.Parse(source.InstrumentUid),
            Balance = source.Quantity,
        };

    private static Position[] Convert(
        this Tinkoff.InvestApi.V1.PortfolioPosition[] source)
        => [.. source.Select(Convert)];
}