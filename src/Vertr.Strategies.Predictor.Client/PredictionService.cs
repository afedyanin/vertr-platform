using System.Text;
using Microsoft.Data.Analysis;
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

    public async Task<string?> Predict(StrategyType strategyType, string content)
    {
        var request = new PredictionRequest
        {
            ModelType = strategyType.ToString(),
            CsvContent = content
        };

        var response = await _predictorClient.Predict(request);

        return response?.CsvContent;
    }
    public async Task<DataFrame?> Predict(StrategyType strategyType, DataFrame dataFrame)
    {
        using var writeStream = new MemoryStream();
        DataFrame.SaveCsv(dataFrame, writeStream);
        var csv = Encoding.UTF8.GetString(writeStream.ToArray());
        var resultCsv = await Predict(strategyType, csv);

        if (string.IsNullOrEmpty(resultCsv))
        {
            return null;
        }

        using var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(resultCsv));
        var df = DataFrame.LoadCsv(csvStream);

        return df;
    }
}
