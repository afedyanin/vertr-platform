using System.Text.Json.Serialization;

namespace Vertr.CommandLine.Predictor.Client.Models;

public record class PredictionResult
{
    [JsonPropertyName("time_utc")]
    public DateTime TimeUtc { get; init; }

    [JsonPropertyName("signal")]
    public int? Signal { get; init; }

    [JsonPropertyName("predicted_price")]
    public decimal? PredictedPrice { get; init; }
}