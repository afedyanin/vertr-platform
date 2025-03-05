using Vertr.Domain.Enums;

namespace Vertr.Adapters.Tinvest.Converters;
internal static class HistoricCandleConverter
{
    public static IEnumerable<Domain.HistoricCandle> Convert(
        this IEnumerable<Tinkoff.InvestApi.V1.HistoricCandle> source,
        string symbol,
        CandleInterval candleInterval)
    {
        foreach(var item in source)
        {
            yield return item.Convert(symbol, candleInterval);
        }
    }

    public static Domain.HistoricCandle Convert(
        this Tinkoff.InvestApi.V1.HistoricCandle source,
        string symbol,
        CandleInterval candleInterval)
        => new Domain.HistoricCandle
        {
            TimeUtc = source.Time.ToDateTime(),
            Interval = candleInterval,
            Symbol = symbol,
            Open = source.Open,
            Close = source.Close,
            High = source.High,
            Low = source.Low,
            Volume = source.Volume,
            IsCompleted = source.IsComplete,
            CandleSource = (int)source.CandleSource,
        };
}
