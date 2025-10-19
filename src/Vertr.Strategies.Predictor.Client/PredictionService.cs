using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Extensions;
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

    public async Task<PredictionResult> Predict(StrategyType strategyType, string content)
    {
        var request = new PredictionRequest
        {
            ModelType = strategyType.ToString(),
            CsvContent = content
        };

        var response = await _predictorClient.Predict(request);

        return new PredictionResult(response?.Result);
    }
    public async Task<PredictionResult> Predict(StrategyType strategyType, IEnumerable<Candle> candles)
    {
        var csv = candles.ToCsv();

        if (string.IsNullOrEmpty(csv))
        {
            return null;
        }

        var result = await Predict(strategyType, csv);
        return result;
    }
}
