using Vertr.Strategies.Contracts;
using Vertr.Strategies.Contracts.Interfaces;
using Vertr.Strategies.Predictor.Client.Requests;

namespace Vertr.Strategies.Predictor.Client;

internal class PredictionService : IPredictionService
{
    private readonly IPredictorClient _predictorClient;

    public PredictionService(IPredictorClient predictorClient)
    {
        _predictorClient = predictorClient;
    }

    public async Task<string> Predict(StrategyType strategyType, string content)
    {
        var request = new PredictionRequest
        {
            ModelType = strategyType.ToString(),
            CsvContent = content
        };

        var response = await _predictorClient.Predict(request);

        return response?.CsvContent ?? string.Empty;
    }
}
