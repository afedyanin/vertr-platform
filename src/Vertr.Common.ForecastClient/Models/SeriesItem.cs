using System.Text.Json.Serialization;

namespace Vertr.Common.ForecastClient.Models;

public record class SeriesItem
{
    [JsonPropertyName("unique_id")]
    public required string Ticker { get; set; }

    [JsonPropertyName("ds")]
    public required string Time { get; set; }

    [JsonPropertyName("y")]
    public decimal Value { get; set; }
}
