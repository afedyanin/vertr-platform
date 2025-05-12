using Vertr.TinvestGateway.Contracts;

namespace Vertr.TinvestGateway.Converters;

internal static class CandleConverter
{
    public static Candle[] Convert(
        this Tinkoff.InvestApi.V1.HistoricCandle[] source,
        string symbol,
        CandleInterval candleInterval)
        => [.. source.Select(c => c.Convert(symbol, candleInterval))];

    public static Candle Convert(
        this Tinkoff.InvestApi.V1.HistoricCandle source,
        string symbol,
        CandleInterval candleInterval)
        => new Candle(
            source.Time.ToDateTime(),
            symbol,
            candleInterval,
            source.Open,
            source.Close,
            source.High,
            source.Low,
            source.Volume,
            source.IsComplete
            );
}
