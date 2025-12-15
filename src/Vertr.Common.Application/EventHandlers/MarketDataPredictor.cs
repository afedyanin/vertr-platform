using Disruptor;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.EventHandlers;

internal sealed class MarketDataPredictor : IAsyncBatchEventHandler<CandlestickReceivedEvent>
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

    public async ValueTask OnBatch(EventBatch<CandlestickReceivedEvent> batch, long sequence)
    {
        try
        {
            foreach (var data in batch)
            {
                var instrumentId = data.Candle!.InstrumentId;
                var candles = await GetCandles(instrumentId);
                var predictors = _portfolioRepository.GetPredictors().Keys;
                _logger.LogDebug("{CandlesCount} candles retrived for predictors. InstrumentId={InstrumentId} Predictors={Predictors}", candles.Length, instrumentId, string.Join(',', predictors));

                var predictions = await _predictorClient.Predict([.. predictors], candles);

                foreach (var prediction in predictions)
                {
                    data.Predictions.Add(prediction);
                }

                _logger.LogInformation("Candle processed: CandleTime={CandleTime}", data.Candle.TimeUtc);
            }

            _logger.LogInformation("MarketDataPredictor executed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MarketDataPredictor error. Message={Message}", ex.Message);
        }
    }

    private async Task<Candle[]> GetCandles(Guid instrumentId)
    {
        if (_candleRepository.GetCount(instrumentId) < _candleRepository.MaxCandlesCount)
        {
            var historicCandles = await _gatewayClient.GetCandles(instrumentId);
            _candleRepository.Load(historicCandles);

            _logger.LogInformation("Historic candles loaded for InstrumentId={InstrumentId} Count={CandlesCount}", instrumentId, historicCandles.Length);
        }

        return _candleRepository.Get(instrumentId);
    }
}
