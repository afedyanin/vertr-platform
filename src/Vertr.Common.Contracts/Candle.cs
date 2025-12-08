namespace Vertr.Common.Contracts;

public record class Candle(
    Guid InstrumentId,
    DateTime TimeUtc,
    decimal Open,
    decimal Close,
    decimal High,
    decimal Low,
    decimal Volume)
{

    public static Candle FromCandlestick(Candlestick candlestick, Guid instrumentId)
        => new Candle(instrumentId, candlestick.GetDateTime(), candlestick.Open, candlestick.Close, candlestick.High, candlestick.Low, candlestick.Volume);
    /*
    // TODO: Convert to extension property (C#14)
    public int Date => int.Parse(TimeUtc.ToString("yyMMdd"));

    // TODO: Convert to extension property (C#14)
    public int Time => int.Parse(TimeUtc.ToString("HHmmss"));
    */
}