using Vertr.Adapters.Prediction.Converters;
using Vertr.Adapters.Prediction.Extensions;
using Vertr.Adapters.Prediction.Models;

using Vertr.Domain.Enums;
using Vertr.Domain.Ports;
using Vertr.Domain.Settings;

namespace Vertr.Adapters.Prediction;

internal class PredictionService : IPredictionService
{
    private readonly IPredictionApi _predictionApi;

    public PredictionService(IPredictionApi predictionApi)
    {
        _predictionApi = predictionApi;
    }

    public async Task<IEnumerable<(DateTime, TradeAction)>> Predict(
        StrategySettings strategySettings,
        int candlesCount = 20,
        bool completedOnly = false,
        string candleSource = "tinvest")
    {
        var request = new PredictionRequest
        {
            Symbol = strategySettings.Symbol,
            Interval = (int)strategySettings.Interval,
            Predictor = strategySettings.PredictorType.GetName(),
            Algo = strategySettings.Sb3Algo.GetName(),
            CandlesCount = candlesCount,
            CompletedCandelsOnly = completedOnly,
            CandlesSource = candleSource,
        };

        var response = await _predictionApi.Predict(request);

        return response.Convert();
    }
}
