using Vertr.TinvestGateway.Contracts;

namespace Vertr.TinvestGateway.Converters;

internal static class PositionConverter
{
    public static PositionsResponse Convert(this Tinkoff.InvestApi.V1.PositionsResponse source)
        => new PositionsResponse
        {
            Securities = source.Securities.ToArray().Convert(),
            Futures = source.Futures.ToArray().Convert(),
            Options = source.Options.ToArray().Convert(),
            Money = source.Money.ToArray().Convert(),
            Blocked = source.Blocked.ToArray().Convert(),
        };

    public static Position Convert(this Tinkoff.InvestApi.V1.PositionsSecurities source)
        => new Position(
            source.PositionUid,
            source.InstrumentUid,
            source.Blocked,
            source.Balance);
    public static Position[] Convert(this Tinkoff.InvestApi.V1.PositionsSecurities[] source)
        => source.Select(Convert).ToArray();

    public static Position Convert(this Tinkoff.InvestApi.V1.PositionsFutures source)
        => new Position(
            source.PositionUid,
            source.InstrumentUid,
            source.Blocked,
            source.Balance);
    public static Position[] Convert(this Tinkoff.InvestApi.V1.PositionsFutures[] source)
        => source.Select(Convert).ToArray();

    public static Position Convert(this Tinkoff.InvestApi.V1.PositionsOptions source)
        => new Position(
            source.PositionUid,
            source.InstrumentUid,
            source.Blocked,
            source.Balance);

    public static Position[] Convert(this Tinkoff.InvestApi.V1.PositionsOptions[] source)
        => source.Select(Convert).ToArray();

    public static Position Convert(this Tinkoff.InvestApi.V1.PortfolioPosition source)
        => new Position(
            source.PositionUid,
            source.InstrumentUid,
            source.BlockedLots == null ? 0L : source.BlockedLots.Units,
            source.Quantity == null ? 0L : source.Quantity.Units);

    public static Position[] Convert(this Tinkoff.InvestApi.V1.PortfolioPosition[] source)
        => source.Select(Convert).ToArray();
}
