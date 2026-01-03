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

    public async ValueTask OnEvent(CandleReceivedEvent data)
    {
        var instrumentId = data.Instrument!.Id;
        var candles = _candleRepository.Get(instrumentId);

        if (candles.Length <= 0)
        {
            _logger.LogWarning("#{Sequence} MarketDataPredictor has no candles to prediction.", data.Sequence);
            return;
        }

        data.PredictionSampleInfo = new PredictionSampleInfo
        {
            Count = candles.Length,
            From = candles.First().TimeUtc,
            To = candles.Last().TimeUtc,
            ClosePriceStats = candles.Select(c => c.Close).GetStats()
        };

        var predictors = _portfolioRepository.GetAll().Keys;
        var predictions = await _predictorClient.Predict([.. predictors], candles);

        foreach (var prediction in predictions)
        {
            data.Predictions.Add(prediction);
        }

        _logger.LogDebug("#{Sequence} MarketDataPredictor executed.", data.Sequence);
    }
}
