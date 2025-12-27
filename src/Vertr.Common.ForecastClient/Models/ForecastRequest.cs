using System.Text.Json.Serialization;

namespace Vertr.Common.ForecastClient.Models;

public record class ForecastRequest
{
    [JsonPropertyName("models")]
    public string[] Models { get; set; } = [];

    [JsonPropertyName("series")]
    public SeriesItem[] Series { get; set; } = [];
}
