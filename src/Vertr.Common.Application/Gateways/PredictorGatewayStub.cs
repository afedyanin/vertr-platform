using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Gateways;

internal sealed class PredictorGatewayStub : IPredictorGateway
{
    public Task<Prediction[]> Predict(string[] predictors, Candle[] candles)
    {
        var predictions = new List<Prediction>();

        foreach (var predictor in predictors)
        {
            var p = new Prediction
            {
                Predictor = predictor,
                InstrumentId = candles.First().InstrumentId,
                Price = GetRandomPrice(candles),
            };

            predictions.Add(p);
        }

        return Task.FromResult(predictions.ToArray());
    }

    private decimal GetRandomPrice(Candle[] candles)
    {
        var prices = candles.Select(c => c.Close).ToArray();

        var count = prices.Length;
        var avg = prices.Average();
        var sum = prices.Sum(d => (d - avg) * (d - avg));
        var dev = Math.Sqrt((double)sum / count);
        var diff = dev * GetRandomDirection();

        return (decimal)((double)avg + diff);
    }

    private static int GetRandomDirection()
        => Random.Shared.Next(0, 2) > 0 ? 1 : -1;

}
