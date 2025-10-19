using System.Text.Json.Serialization;

namespace Vertr.Strategies.Predictor.Client.Requests;

public class PredictionRequest
{
    [JsonPropertyName("model_type")]
    public required string ModelType { get; set; }

    [JsonPropertyName("csv")]
    public required string CsvContent { get; set; }
}

public class PredictionResponse
{
    [JsonPropertyName("result")]
    public Dictionary<string, object>? Result { get; set; }
}
