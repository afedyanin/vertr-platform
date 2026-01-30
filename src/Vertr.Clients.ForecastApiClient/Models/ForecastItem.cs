using System.Text.Json.Serialization;

namespace Vertr.Clients.ForecastApiClient.Models;

public record class ForecastItem
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("unique_id")]
    public string? Ticker { get; set; }

    [JsonPropertyName("ds")]
    public DateTime Time { get; set; }

    [JsonPropertyName("value")]
    public decimal Value { get; set; }
}
