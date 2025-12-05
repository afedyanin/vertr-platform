using System.Text.Json;

namespace Vertr.TinvestGateway.Contracts.MarketData;

public record class Instrument
{
    public Guid Id { get; set; }
    public required string ClassCode { get; set; } = string.Empty;
    public required string Ticker { get; set; } = string.Empty;
    public string? InstrumentType { get; set; }
    public string? Name { get; set; }
    public string? Currency { get; set; }
    public decimal? LotSize { get; set; }

    public string ToJson() => JsonSerializer.Serialize(this, JsonOptions.DefaultOptions);

    public static Instrument? FromJson(string json) => JsonSerializer.Deserialize<Instrument>(json, JsonOptions.DefaultOptions);
}