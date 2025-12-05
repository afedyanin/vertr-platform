using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Models.Requests.Predictor;
using Vertr.CommandLine.Predictor.Client.Convertors;

namespace Vertr.CommandLine.Predictor.Client;

internal class PredictionService : IPredictionService
{
    private readonly IPredictorClient _predictorClient;

    public PredictionService(IPredictorClient predictorClient)
    {
        _predictorClient = predictorClient;
    }

    public async Task<Dictionary<string, object>> Predict(
        DateTime time,
        string symbol,
        PredictorType predictor,
        Dictionary<string, object> marketData)
    {
        return predictor switch
        {
            PredictorType.None => [],
            PredictorType.AutoArima => await AutoArima(marketData),
            PredictorType.Garch => await Garch(marketData),
            PredictorType.HistoricAverage => await HistoricAverage(marketData),
            PredictorType.RandomWalkWithDrift => await RandomWalkWithDrift(marketData),
            PredictorType.Naive => await Naive(marketData),
            _ => throw new NotImplementedException($"Predictor type={predictor} is not implemented."),
        };
    }

    private async Task<Dictionary<string, object>> AutoArima(Dictionary<string, object> marketData)
    {
        var candles = GetCandles(marketData);

        if (candles == null)
        {
            return [];
        }

        var response = await _predictorClient.AutoArima(candles.ToRequest());
        var res = response.ToDictionary();
        return res;
    }

    private async Task<Dictionary<string, object>> Garch(Dictionary<string, object> marketData)
    {
        var candles = GetCandles(marketData);

        if (candles == null)
        {
            return [];
        }

        var response = await _predictorClient.Garch(candles.ToRequest());
        var res = response.ToDictionary();
        return res;
    }

    private async Task<Dictionary<string, object>> RandomWalkWithDrift(Dictionary<string, object> marketData)
    {
        var candles = GetCandles(marketData);

        if (candles == null)
        {
            return [];
        }

        var response = await _predictorClient.RandomWalkWithDrift(candles.ToRequest());
        var res = response.ToDictionary();
        return res;
    }

    private async Task<Dictionary<string, object>> Naive(Dictionary<string, object> marketData)
    {
        var candles = GetCandles(marketData);

        if (candles == null)
        {
            return [];
        }

        var response = await _predictorClient.Naive(candles.ToRequest());
        var res = response.ToDictionary();
        return res;
    }

    private async Task<Dictionary<string, object>> HistoricAverage(Dictionary<string, object> marketData)
    {
        var candles = GetCandles(marketData);

        if (candles == null)
        {
            return [];
        }

        var response = await _predictorClient.HistoricAverage(candles.ToRequest());
        var res = response.ToDictionary();
        return res;
    }

    private static Candle[]? GetCandles(Dictionary<string, object> marketData)
    {
        marketData.TryGetValue(PredictionContextKeys.Candles, out var candlesEntry);
        return candlesEntry as Candle[];
    }
}