using Vertr.Adapters.Prediction.Converters;
using Vertr.Adapters.Prediction.Models;
using Vertr.Domain;
using Vertr.Domain.Enums;
using Vertr.Domain.Ports;

namespace Vertr.Adapters.Prediction;

internal class PredictionService : IPredictionService
{
    private readonly IPredictionApi _predictionApi;

    public PredictionService(IPredictionApi predictionApi)
    {
        _predictionApi = predictionApi;
    }

    public async Task<IEnumerable<(DateTime, TradeAction)>> Predict(
        string symbol,
        CandleInterval interval,
        PredictorType predictor,
        Sb3Algo? algo = null,
        int candlesCount = 20,
        bool completedOnly = false,
        string candleSource = "tinvest")
    {
        var request = new PredictionRequest
        {
            Symbol = symbol,
            Interval = (int)interval,
            Predictor = predictor.Name,
            Algo = algo != null ? algo.Name : string.Empty,
            CandlesCount = candlesCount,
            CompletedCandelsOnly = completedOnly,
            CandlesSource = candleSource,
        };

        var response = await _predictionApi.Predict(request);

        return response.Convert();
    }
}
