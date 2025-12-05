using System.Text.Json;

namespace Vertr.TinvestGateway.Contracts.MarketData;

public readonly record struct Candlestick
{
    public long Time { get; }
    public decimal Open { get; }
    public decimal High { get; }
    public decimal Low { get; }
    public decimal Close { get; }
    public decimal Volume { get; }

    public DateTime GetDateTime() => new(Time);

    public Candlestick(
        DateTime time,
        decimal open,
        decimal high,
        decimal low,
        decimal close,
        decimal volume) : this(
            time.Ticks,
            open,
            high,
            low,
            close,
            volume)
    {
    }

    private Candlestick(
        long time,
        decimal open,
        decimal high,
        decimal low,
        decimal close,
        decimal volume)
    {
        Time = time;
        Open = open;
        High = high;
        Low = low;
        Close = close;
        Volume = volume;
    }

    public string ToJson()
    {
        var items = new decimal[] { Time, Open, High, Low, Close, Volume };
        return JsonSerializer.Serialize(items, JsonOptions.DefaultOptions);
    }

    public static Candlestick? FromJson(string jsonString)
    {
        var items = JsonSerializer.Deserialize<decimal[]>(jsonString, JsonOptions.DefaultOptions);

        return items == null || items.Length < 6 ? null : new Candlestick((long)items[0], items[1], items[2], items[3], items[4], items[5]);
    }
}