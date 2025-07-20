using Vertr.MarketData.Contracts;

namespace Vertr.TinvestGateway.Application.Converters;

internal static class CandleConverter
{
    public static Candle[] Convert(this Tinkoff.InvestApi.V1.HistoricCandle[] source, Guid instrumentId)
        => [.. source.Select(s => s.Convert(instrumentId))];

    public static Candle Convert(this Tinkoff.InvestApi.V1.HistoricCandle source, Guid instrumentId)
        => new Candle(
            source.Time.ToDateTime(),
            source.Open,
            source.Close,
            source.High,
            source.Low,
            source.Volume,
            instrumentId);

    public static Candle Convert(this Tinkoff.InvestApi.V1.Candle source, Guid instrumentId)
        => new Candle(
            source.Time.ToDateTime(),
            source.Open,
            source.Close,
            source.High,
            source.Low,
            source.Volume,
            instrumentId);
}
