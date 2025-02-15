using Refit;
using Vertr.Adapters.Prediction.Models;

namespace Vertr.Adapters.Prediction;

// https://github.com/reactiveui/refit
internal interface IPredictionApi
{
    [Post("/prediction/predict")]
    Task<PredictionResponse> Predict(PredictionRequest predictionRequest);
}
