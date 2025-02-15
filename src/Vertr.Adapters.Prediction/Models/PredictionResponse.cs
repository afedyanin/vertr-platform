namespace Vertr.Adapters.Prediction.Models;

internal record class PredictionResponse
{
    public DateTime[] Time { get; set; } = [];

    public Action[] Action { get; set; } = [];
}
