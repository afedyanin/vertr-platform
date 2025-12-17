using Disruptor;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.Extensions;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.EventHandlers;

internal sealed class MarketDataPredictor : IEventHandler<CandlestickReceivedEvent>
{
    private const int ThresholdSigma = 1;
    private const double DefaultThreshold = 0.001;

    private readonly ICandlesLocalStorage _candleRepository;
    private readonly IPortfoliosLocalStorage _portfolioRepository;
    private readonly ITradingGateway _gatewayClient;
    private readonly IPredictorGateway _predictorClient;
    private readonly IMarketQuoteProvider? _marketQuoteProvider;

    private readonly ILogger<MarketDataPredictor> _logger;

    public MarketDataPredictor(
        ICandlesLocalStorage candleRepository,
        IPortfoliosLocalStorage portfolioRepository,
        ITradingGateway gatewayClient,
        IPredictorGateway predictorClient,
        IMarketQuoteProvider? marketQuoteProvider,
        ILogger<MarketDataPredictor> logger)
    {
        _candleRepository = candleRepository;
        _portfolioRepository = portfolioRepository;
        _gatewayClient = gatewayClient;
        _predictorClient = predictorClient;
        _marketQuoteProvider = marketQuoteProvider;
        _logger = logger;
    }

    public void OnEvent(CandlestickReceivedEvent data, long sequence, bool endOfBatch)
    {
        try
        {
            var instrumentId = data.Candle!.InstrumentId;
            var candles = GetCandles(instrumentId);

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

            data.MarketQuote = _marketQuoteProvider?.GetMarketQuote(instrumentId) ?? GetMarketQuote(data.Candle);

            // TODO: Use fixed or floating threshold? - move to Hyperparams
            // var stats = data.PredictionSampleInfo.ClosePriceStats;
            // data.PriceThreshold = stats.StdDev / stats.Mean * ThresholdSigma;
            data.PriceThreshold = DefaultThreshold * ThresholdSigma;

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

    private Candle[] GetCandles(Guid instrumentId)
    {
        if (_candleRepository.GetCount(instrumentId) < _candleRepository.CandlesBufferLength)
        {
            var historicCandles = _gatewayClient.GetCandles(instrumentId).GetAwaiter().GetResult();
            _candleRepository.Load(historicCandles);

            _logger.LogInformation("({CandlesCount}) historic candles loaded for InstrumentId={InstrumentId}", historicCandles.Length, instrumentId);
        }

        return _candleRepository.Get(instrumentId);
    }

    private static Quote? GetMarketQuote(Candle last)
    {
        var prices = new decimal[]
        {
            last.Open,
            last.Close,
            last.High,
            last.Low
        };

        return new Quote
        {
            Bid = prices.Min(),
            Ask = prices.Max()
        };
    }
}
