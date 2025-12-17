using Disruptor;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.Extensions;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.EventHandlers;

internal sealed class MarketDataPredictor : IEventHandler<CandleReceivedEvent>
{
    private readonly ICandlesLocalStorage _candleRepository;
    private readonly IPortfoliosLocalStorage _portfolioRepository;
    private readonly IPredictorGateway _predictorClient;
    private readonly ILogger<MarketDataPredictor> _logger;

    public MarketDataPredictor(
        ICandlesLocalStorage candleRepository,
        IPortfoliosLocalStorage portfolioRepository,
        IPredictorGateway predictorClient,
        ILogger<MarketDataPredictor> logger)
    {
        _candleRepository = candleRepository;
        _portfolioRepository = portfolioRepository;
        _predictorClient = predictorClient;
        _logger = logger;
    }

    public void OnEvent(CandleReceivedEvent data, long sequence, bool endOfBatch)
    {
        try
        {
            var instrumentId = data.Instrument!.Id;
            var candles = _candleRepository.Get(instrumentId);

            if (candles.Length <= 0)
            {
                _logger.LogWarning("#{Sequence} MarketDataPredictor has no candles to prediction.", sequence);
                return;
            }

            var predictors = _portfolioRepository.GetPredictors().Keys;

            data.PredictionSampleInfo = new PredictionSampleInfo
            {
                Count = candles.Length,
                From = candles.First().TimeUtc,
                To = candles.Last().TimeUtc,
                ClosePriceStats = candles.Select(c => c.Close).GetPriceStats()
            };

            var predictions = _predictorClient.Predict([.. predictors], candles).GetAwaiter().GetResult();

            foreach (var prediction in predictions)
            {
                data.Predictions.Add(prediction);
            }

            _logger.LogDebug("#{Sequence} MarketDataPredictor executed.", sequence);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "#{Sequence} MarketDataPredictor error. Message={Message}", ex.Message, sequence);
        }
    }
}
