using System.Text.Json;

namespace Vertr.Common.Contracts;

public record class FutureInfo(
    string ClassCode,
    string Ticker,
    DateOnly ExpDate,
    DateOnly LastTradeDate,
    decimal LotSize,
    decimal PriceStep)
{
    public string ToJson()
        => JsonSerializer.Serialize(this, JsonOptions.DefaultOptions);

    public static FutureInfo? FromJson(string jsonString)
        => JsonSerializer.Deserialize<FutureInfo>(jsonString, JsonOptions.DefaultOptions);
}
