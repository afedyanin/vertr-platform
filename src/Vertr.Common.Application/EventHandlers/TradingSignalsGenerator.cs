using Disruptor;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Services;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.EventHandlers;

internal sealed class TradingSignalsGenerator : IEventHandler<CandlestickReceivedEvent>
{
    private const double DefaultThreshold = 0.001;
    private const int ThresholdSigma = 1;

    private readonly ILogger<TradingSignalsGenerator> _logger;
    private readonly IOrderBookRepository _orderBookRepository;
    private readonly ICandleRepository _candleRepository;

    public TradingSignalsGenerator(
        IOrderBookRepository orderBookRepository,
        ICandleRepository candleRepository,
        ILogger<TradingSignalsGenerator> logger)
    {
        _orderBookRepository = orderBookRepository;
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
        if (predictedPrice == null)
        {
            return TradingDirection.Hold;
        }

        var quote = _orderBookRepository.GetMarketQuote(instrumentId);

        if (quote == null)
        {
            return TradingDirection.Hold;
        }

        var threshold = GetThreshold(instrumentId);

        // цена будет выше минимальной цены предложения
        var askDelta = predictedPrice.Value - quote.Value.Ask;
        if ((double)askDelta > threshold)
        {
            return TradingDirection.Buy;
        }

        // цена будет ниже максимальной цены спроса
        var bidDelta = quote.Value.Bid - predictedPrice.Value;
        if ((double)bidDelta > threshold)
        {
            return TradingDirection.Sell;
        }

        return TradingDirection.Hold;
    }

    private double GetThreshold(Guid instrumentId)
    {
        var stats = _candleRepository.GetStats(instrumentId);
        var th = stats == null ? DefaultThreshold : stats.Value.StdDev;
        return th * ThresholdSigma;
    }
}
