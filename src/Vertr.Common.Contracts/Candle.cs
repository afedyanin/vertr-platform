using System.Text.Json;

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
    public string ToJson()
        => JsonSerializer.Serialize(this, JsonOptions.DefaultOptions);

    public static Candle? FromJson(string jsonString)
        => JsonSerializer.Deserialize<Candle>(jsonString, JsonOptions.DefaultOptions);
}
