using Vertr.Common.Contracts;
using Vertr.Strategies.CandlesForecast.Models;

namespace Vertr.Strategies.CandlesForecast.Abstractions;

internal interface IPredictorGateway
{
    public Task<Prediction[]> Predict(
        string[] predictors,
        Candle[] candles);
}
