using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Models.Requests.Orders;
using Vertr.CommandLine.Models.Requests.Predictor;

namespace Vertr.CommandLine.Application.Handlers.Predictor;

public class PredictionHandler : IRequestHandler<PredictionRequest, PredictionResponse>
{
    private readonly IMarketDataService _marketDataService;
    private readonly IPredictionService _predictionService;

    public PredictionHandler(
        IMarketDataService marketDataService,
        IPredictionService predictionService)
    {
        _marketDataService = marketDataService;
        _predictionService = predictionService;
    }
    public async Task<PredictionResponse> Handle(PredictionRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var candles = await _marketDataService.GetCandles(request.Symbol, request.Time, request.CandlesCount);

            if (candles == null || candles.Length <= 0)
            {
                return new PredictionResponse
                {
                    Message = $"Cannot find any candles for symbol={request.Symbol} at time={request.Time:O}"
                };
            }

            var marketData = new Dictionary<string, object>
            {
                [PredictionContextKeys.Candles] = candles
            };

            var predictionResult = await _predictionService.Predict(request.Time, request.Symbol, request.Predictor, marketData);

            predictionResult.TryGetValue(PredictionContextKeys.PredictedPrice, out var price);
            predictionResult.TryGetValue(PredictionContextKeys.LastCandle, out var lastCandle);
            predictionResult.TryGetValue(PredictionContextKeys.Signal, out var signal);

            var response = new PredictionResponse()
            {
                PredictedPrice = price as decimal?,
                LastCandle = lastCandle as Candle,
                Signal = signal as Direction?
            };

            return response;

        }
        catch (Exception ex)
        {
            return new PredictionResponse
            {
                Exception = ex,
                Message = $"Prediction error. Message={ex.Message}"
            };
        }
    }
}