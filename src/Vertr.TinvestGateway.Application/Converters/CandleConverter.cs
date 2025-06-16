using Vertr.TinvestGateway.Contracts;

namespace Vertr.TinvestGateway.Application.Converters;

internal static class CandleConverter
{
    public static Candle[] Convert(this Tinkoff.InvestApi.V1.HistoricCandle[] source)
        => [.. source.Select(Convert)];

    public static Candle Convert(this Tinkoff.InvestApi.V1.HistoricCandle source)
        => new Candle(
            source.Time.ToDateTime(),
            source.Open,
            source.Close,
            source.High,
            source.Low,
            source.Volume);

    public static Candle Convert(this Tinkoff.InvestApi.V1.Candle source)
        => new Candle(
            source.Time.ToDateTime(),
            source.Open,
            source.Close,
            source.High,
            source.Low,
            source.Volume);
}
