using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Clients;

public interface IPredictorClient
{
    public Task<Prediction[]> Predict(string[] predictors, Candle[] candles);
}
