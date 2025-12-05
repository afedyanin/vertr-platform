using Vertr.CommandLine.Models.Requests.Predictor;

namespace Vertr.CommandLine.Models.Abstracttions;

public interface IPredictionService
{
    public Task<Dictionary<string, object>> Predict(
        DateTime time,
        string symbol,
        PredictorType predictor,
        Dictionary<string, object> marketData);
}