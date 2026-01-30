using Vertr.Common.Contracts;

namespace Vertr.Strategies.CandlesForecast.Abstractions;

internal interface IPredictorGateway
{
    public Task<Prediction[]> Predict(
        string[] predictors,
        Candle[] candles);
}
