using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Models.Requests.Predictor;

namespace Vertr.CommandLine.Application.Services;

internal class SimulatedPredictionService : IPredictionService
{
    public Task<Dictionary<string, object>> Predict(
        DateTime time,
        string symbol,
        PredictorType predictor,
        Dictionary<string, object> marketData)
    {
        var res = new Dictionary<string, object>();

        marketData.TryGetValue(PredictionContextKeys.Candles, out var candlesEntry);

        if (candlesEntry is not Candle[] candles)
        {
            return Task.FromResult(res);
        }

        var lastCandle = candles.OrderBy(c => c.TimeUtc).LastOrDefault();

        if (lastCandle == null)
        {
            return Task.FromResult(res);
        }

        res[PredictionContextKeys.PredictedPrice] = lastCandle.Close;
        res[PredictionContextKeys.LastCandle] = lastCandle;

        return Task.FromResult(res);
    }
}