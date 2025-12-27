using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.Services;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Gateways;

internal sealed class PredictorGatewayStub : IPredictorGateway
{
    public Task<Prediction[]> Predict(
        string[] predictors,
        Candle[] candles)
    {
        var predictions = new List<Prediction>();

        if (candles.Length <= 0)
        {
            return Task.FromResult(predictions.ToArray());
        }

        foreach (var predictor in predictors)
        {
            var p = new Prediction
            {
                Predictor = predictor,
                InstrumentId = candles.First().InstrumentId,
                Value = RandomCandleGenerator.GetNextValue(candles.Last().Close, 0.05),
            };

            predictions.Add(p);
        }

        return Task.FromResult(predictions.ToArray());
    }
}
