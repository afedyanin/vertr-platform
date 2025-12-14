using Vertr.Common.Contracts;

namespace Vertr.TinvestGateway.Converters;

public static class CandleConverter
{
    public static Candlestick[] ToCandlesticks(this Tinkoff.InvestApi.V1.HistoricCandle[] source)
        => [.. source.Select(ToCandlestick)];

    public static Candlestick ToCandlestick(this Tinkoff.InvestApi.V1.HistoricCandle source)
        => new Candlestick(
            source.Time.ToDateTime(),
            source.High,
            source.Low,
            source.Open,
            source.Close,
            source.Volume);

    public static Candlestick ToCandlestick(
        this Tinkoff.InvestApi.V1.Candle source)
        => new Candlestick(
            source.Time.ToDateTime(),
            source.High,
            source.Low,
            source.Open,
            source.Close,
            source.Volume);
}