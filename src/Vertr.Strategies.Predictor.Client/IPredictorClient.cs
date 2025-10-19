using Refit;
using Vertr.Strategies.Predictor.Client.Requests;

namespace Vertr.Strategies.Predictor.Client;

public interface IPredictorClient
{
    [Post("/prediction/predict")]
    public Task<PredictionResponse> Predict(PredictionRequest request);
}
