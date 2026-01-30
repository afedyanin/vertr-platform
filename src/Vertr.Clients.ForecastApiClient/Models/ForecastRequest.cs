using System.Text.Json.Serialization;

namespace Vertr.Clients.ForecastApiClient.Models;

public record class ForecastRequest
{
    [JsonPropertyName("models")]
    public string[] Models { get; set; } = [];

    [JsonPropertyName("series")]
    public SeriesItem[] Series { get; set; } = [];
}
