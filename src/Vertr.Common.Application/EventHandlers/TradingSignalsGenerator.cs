using Disruptor;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.EventHandlers;

internal sealed class TradingSignalsGenerator : IEventHandler<CandlestickReceivedEvent>
{
    private const double DefaultThreshold = 0.001;
    private const int ThresholdSigma = 1;

    private readonly ILogger<TradingSignalsGenerator> _logger;
    private readonly IMarketQuoteProvider _marketQuoteProvider;
    private readonly ICandlesLocalStorage _candleRepository;

    public TradingSignalsGenerator(
        IMarketQuoteProvider marketQuoteProvider,
        ICandlesLocalStorage candleRepository,
        ILogger<TradingSignalsGenerator> logger)
    {
        _marketQuoteProvider = marketQuoteProvider;
        _candleRepository = candleRepository;
        _logger = logger;
    }

    public void OnEvent(CandlestickReceivedEvent data, long sequence, bool endOfBatch)
    {
        foreach (var prediction in data.Predictions)
        {
            var direction = GetTradingDirection(prediction.InstrumentId, prediction.Price);

            if (direction == TradingDirection.Hold)
            {
                continue;
            }

            var signal = new TradingSignal
            {
                Predictor = prediction.Predictor,
                InstrumentId = prediction.InstrumentId,
                Direction = direction,
            };

            _logger.LogInformation("Trading signal created for InstrumentId={InstrumentId} Predictor={Predictor} Price={Price} Direction={Direction}",
                prediction.InstrumentId, prediction.Predictor, prediction.Price, direction);

            data.TradingSignals.Add(signal);
        }

        _logger.LogInformation("TradingSignalsGenerator executed.");
    }

    private TradingDirection GetTradingDirection(Guid instrumentId, decimal? predictedPrice)
    {
        if (predictedPrice == null || predictedPrice.Value == default)
        {
            _logger.LogInformation("PredictedPrice is not defined. TradingDirection: Hold.");
            return TradingDirection.Hold;
        }

        var quote = _marketQuoteProvider.GetMarketQuote(instrumentId);

        if (quote == null)
        {
            _logger.LogInformation("MarketQuote is NULL. TradingDirection: Hold.");
            return TradingDirection.Hold;
        }

        var threshold = GetThreshold(instrumentId);
        _logger.LogInformation("Trading signal threshold for InstrumentId={InstrumentId} Threshold={Threshold}", instrumentId, threshold);

        // цена будет выше минимальной цены предложения
        var askDelta = (double)(predictedPrice.Value - quote.Value.Ask);
        _logger.LogInformation("AskDelta={AskDelta} Threshold={Threshold}", askDelta, threshold);

        if (askDelta >= threshold)
        {
            return TradingDirection.Buy;
        }

        // цена будет ниже максимальной цены спроса
        var bidDelta = (double)(quote.Value.Bid - predictedPrice.Value);
        _logger.LogInformation("BidDelta={BidDelta} Threshold={Threshold}", bidDelta, threshold);
        if (bidDelta >= threshold)
        {
            return TradingDirection.Sell;
        }

        return TradingDirection.Hold;
    }

    private double GetThreshold(Guid instrumentId)
    {
        var stats = _candleRepository.GetStats(instrumentId);
        _logger.LogInformation("Stats={Stats}", stats);

        var th = stats == null ? DefaultThreshold : stats.Value.StdDev;
        return th * ThresholdSigma;
    }
}
