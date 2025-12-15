using Disruptor;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.EventHandlers;

internal sealed class MarketDataPredictor : IEventHandler<CandlestickReceivedEvent>
{
    private readonly ICandlesLocalStorage _candleRepository;
    private readonly IPortfoliosLocalStorage _portfolioRepository;
    private readonly ITradingGateway _gatewayClient;
    private readonly IPredictorGateway _predictorClient;

    private readonly ILogger<MarketDataPredictor> _logger;

    public MarketDataPredictor(
        ICandlesLocalStorage candleRepository,
        IPortfoliosLocalStorage portfolioRepository,
        ITradingGateway gatewayClient,
        IPredictorGateway predictorClient,
        ILogger<MarketDataPredictor> logger)
    {
        _candleRepository = candleRepository;
        _portfolioRepository = portfolioRepository;
        _gatewayClient = gatewayClient;
        _predictorClient = predictorClient;
        _logger = logger;
    }

    public void OnEvent(CandlestickReceivedEvent data, long sequence, bool endOfBatch)
    {
        try
        {
            var instrumentId = data.Candle!.InstrumentId;
            var candles = GetCandles(instrumentId);
            var predictors = _portfolioRepository.GetPredictors().Keys;

            _logger.LogDebug("{CandlesCount} candles retrived for predictors. InstrumentId={InstrumentId} Predictors={Predictors}", candles.Length, instrumentId, string.Join(',', predictors));

            var predictions = _predictorClient.Predict([.. predictors], candles).GetAwaiter().GetResult();

            foreach (var prediction in predictions)
            {
                data.Predictions.Add(prediction);
            }

            _logger.LogInformation("MarketDataPredictor executed for InstrumentId={InstrumentId} CandleTime={CandleTime} Predictions={Predictions}",
                instrumentId, data.Candle.TimeUtc, predictions.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MarketDataPredictor error. Message={Message}", ex.Message);
        }
    }

    private Candle[] GetCandles(Guid instrumentId)
    {
        if (_candleRepository.GetCount(instrumentId) < _candleRepository.MaxCandlesCount)
        {
            var historicCandles = _gatewayClient.GetCandles(instrumentId).GetAwaiter().GetResult();
            _candleRepository.Load(historicCandles);

            _logger.LogInformation("Historic candles loaded for InstrumentId={InstrumentId} Count={CandlesCount}", instrumentId, historicCandles.Length);
        }

        return _candleRepository.Get(instrumentId);
    }
}
