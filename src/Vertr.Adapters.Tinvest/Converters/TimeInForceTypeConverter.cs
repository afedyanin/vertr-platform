namespace Vertr.Adapters.Tinvest.Converters;

internal static class TimeInForceTypeConverter
{
    public static Tinkoff.InvestApi.V1.TimeInForceType Convert(this Domain.TimeInForceType timeInForceType)
        => timeInForceType switch
        {
            Domain.TimeInForceType.Unspecified => Tinkoff.InvestApi.V1.TimeInForceType.TimeInForceUnspecified,
            Domain.TimeInForceType.Day => Tinkoff.InvestApi.V1.TimeInForceType.TimeInForceDay,
            Domain.TimeInForceType.FillAndKill => Tinkoff.InvestApi.V1.TimeInForceType.TimeInForceFillAndKill,
            Domain.TimeInForceType.FillOrKill => Tinkoff.InvestApi.V1.TimeInForceType.TimeInForceFillOrKill,
            _ => throw new InvalidOperationException($"Unknown TimeInForceType={timeInForceType}"),
        };
}
