using System.Text.Json.Serialization;

namespace Vertr.CommandLine.Predictor.Client.Models;

public record class CandleRequest
{
    [JsonPropertyName("time_utc")]
    public DateTime TimeUtc { get; init; }

    [JsonPropertyName("open")]
    public decimal Open { get; init; }

    [JsonPropertyName("close")]
    public decimal Close { get; init; }

    [JsonPropertyName("high")]
    public decimal High { get; init; }

    [JsonPropertyName("low")]
    public decimal Low { get; init; }

    [JsonPropertyName("volume")]
    public long Volume { get; init; }
}