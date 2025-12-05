using Refit;
using Vertr.CommandLine.Predictor.Client.Models;

namespace Vertr.CommandLine.Predictor.Client;

public interface IPredictorClient
{
    [Post("/stats-forecast/naive")]
    public Task<PredictionResult> Naive(CandleRequest[] candles);

    [Post("/stats-forecast/random-walk-with-drift")]
    public Task<PredictionResult> RandomWalkWithDrift(CandleRequest[] candles);


    [Post("/stats-forecast/historic-average")]
    public Task<PredictionResult> HistoricAverage(CandleRequest[] candles);


    [Post("/stats-forecast/auto-arima")]
    public Task<PredictionResult> AutoArima(CandleRequest[] candles);

    [Post("/stats-forecast/garch")]
    public Task<PredictionResult> Garch(CandleRequest[] candles);
}