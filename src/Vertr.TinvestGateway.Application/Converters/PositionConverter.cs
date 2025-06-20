using Vertr.PortfolioManager.Contracts;

namespace Vertr.TinvestGateway.Application.Converters;

internal static class PositionConverter
{
    public static PositionsResponse Convert(
        this Tinkoff.InvestApi.V1.PositionsResponse source,
        string accountId)
        => new PositionsResponse
        {
            AccountId = accountId,
            Securities = source.Securities.ToArray().Convert(),
            Futures = source.Futures.ToArray().Convert(),
            Options = source.Options.ToArray().Convert(),
            Money = source.Money.ToArray().Convert(),
            Blocked = source.Blocked.ToArray().Convert(),
        };

    public static PositionsResponse Convert(this Tinkoff.InvestApi.V1.PositionData source)
        => new PositionsResponse
        {
            AccountId = source.AccountId,
            Securities = source.Securities.ToArray().Convert(),
            Futures = source.Futures.ToArray().Convert(),
            Options = source.Options.ToArray().Convert(),
            Blocked = source.Money.ToArray().GetBlockedAmount(),
            Money = source.Money.ToArray().GetAvailableAmount(),
        };

    public static Money GetAvailableAmount(this Tinkoff.InvestApi.V1.PositionsMoney source)
        => source.AvailableValue.Convert();
    public static Money[] GetAvailableAmount(this Tinkoff.InvestApi.V1.PositionsMoney[] source)
        => [.. source.Select(v => v.GetAvailableAmount())];

    public static Money GetBlockedAmount(this Tinkoff.InvestApi.V1.PositionsMoney source)
        => source.BlockedValue.Convert();

    public static Money[] GetBlockedAmount(this Tinkoff.InvestApi.V1.PositionsMoney[] source)
        => [.. source.Select(v => v.GetBlockedAmount())];

    public static Position Convert(this Tinkoff.InvestApi.V1.PositionsSecurities source)
        => new Position(
            source.PositionUid,
            source.InstrumentUid,
            source.Blocked,
            source.Balance);
    public static Position[] Convert(this Tinkoff.InvestApi.V1.PositionsSecurities[] source)
        => [.. source.Select(Convert)];

    public static Position Convert(this Tinkoff.InvestApi.V1.PositionsFutures source)
        => new Position(
            source.PositionUid,
            source.InstrumentUid,
            source.Blocked,
            source.Balance);
    public static Position[] Convert(this Tinkoff.InvestApi.V1.PositionsFutures[] source)
        => [.. source.Select(Convert)];

    public static Position Convert(this Tinkoff.InvestApi.V1.PositionsOptions source)
        => new Position(
            source.PositionUid,
            source.InstrumentUid,
            source.Blocked,
            source.Balance);

    public static Position[] Convert(this Tinkoff.InvestApi.V1.PositionsOptions[] source)
        => [.. source.Select(Convert)];

    public static Position Convert(this Tinkoff.InvestApi.V1.PortfolioPosition source)
        => new Position(
            source.PositionUid,
            source.InstrumentUid,
            source.BlockedLots ?? 0M,
            source.Quantity ?? 0M);

    public static Position[] Convert(this Tinkoff.InvestApi.V1.PortfolioPosition[] source)
        => [.. source.Select(Convert)];
}
