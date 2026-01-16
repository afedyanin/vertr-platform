using System.Text.Json;

namespace Vertr.Common.Contracts;

public record class MarketTrade
{
    public DateTime Time { get; set; }

    public Guid InstrumentId { get; set; }

    public long Quantity { get; set; }

    public decimal Price { get; set; }

    public TradingDirection Direction { get; set; }

    public string ToJson() => JsonSerializer.Serialize(this, JsonOptions.DefaultOptions);

    public static MarketTrade? FromJson(string json) => JsonSerializer.Deserialize<MarketTrade>(json, JsonOptions.DefaultOptions);
}
