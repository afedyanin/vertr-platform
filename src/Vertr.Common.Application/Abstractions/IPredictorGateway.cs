using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

internal interface IPredictorGateway
{
    public Task<Prediction[]> Predict(string[] predictors, Candle[] candles);
}
