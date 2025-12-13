using Disruptor;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Clients;
using Vertr.Common.Application.Services;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.EventHandlers;

internal sealed class MarketDataPredictor : IAsyncBatchEventHandler<CandlestickReceivedEvent>
{
    private readonly ICandleRepository _candleRepository;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly ITinvestGatewayClient _gatewayClient;
    private readonly IPredictorClient _predictorClient;

    private readonly ILogger<MarketDataPredictor> _logger;

    public MarketDataPredictor(
        ICandleRepository candleRepository,
        IPortfolioRepository portfolioRepository,
        ITinvestGatewayClient gatewayClient,
        IPredictorClient predictorClient,
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
                var predictions = await _predictorClient.Predict([.. predictors], candles);

                foreach (var prediction in predictions)
                {
                    data.Predictions.Add(prediction);
                }
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
            // TODO: Здесь загрузка не сработает, т.к. есть уже более поздняя свеча
            var historicCandles = await _gatewayClient.GetCandles(instrumentId);
            _candleRepository.Load(historicCandles);
        }

        return _candleRepository.Get(instrumentId);
    }
}
