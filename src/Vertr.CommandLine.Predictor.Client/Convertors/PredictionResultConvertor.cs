using Vertr.CommandLine.Models.Requests.Predictor;
using Vertr.CommandLine.Predictor.Client.Models;

namespace Vertr.CommandLine.Predictor.Client.Convertors;

internal static class PredictionResultConvertor
{
    public static Dictionary<string, object> ToDictionary(this PredictionResult predictionResult)
    {
        var dict = new Dictionary<string, object>();

        if (predictionResult.PredictedPrice.HasValue)
        {
            dict[PredictionContextKeys.PredictedPrice] = predictionResult.PredictedPrice;
        }

        if (predictionResult.Signal.HasValue)
        {
            dict[PredictionContextKeys.Signal] = predictionResult.Signal;
        }

        return dict;
    }
}