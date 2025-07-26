using Vertr.PortfolioManager.Contracts;

namespace Vertr.TinvestGateway.Application.Converters;

internal static class PortfolioConverter
{
    public static Portfolio? Convert(
        this Tinkoff.InvestApi.V1.PortfolioResponse source,
        DateTime createdAt)
    {
        if (source == null)
        {
            return null;
        }

        var res = new Portfolio
        {
            UpdatedAt = createdAt,
            Identity = new PortfolioIdentity(source.AccountId),
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
