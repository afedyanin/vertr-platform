using Disruptor;
using Microsoft.Extensions.Logging;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.EventHandlers;

internal sealed class TradingSignalsGenerator : IEventHandler<CandlestickReceivedEvent>
{
    private readonly ILogger<TradingSignalsGenerator> _logger;

    public TradingSignalsGenerator(
        ILogger<TradingSignalsGenerator> logger)
    {
        _logger = logger;
    }

    public void OnEvent(CandlestickReceivedEvent data, long sequence, bool endOfBatch)
    {
        foreach (var prediction in data.Predictions)
        {
            if (prediction.Price == null || prediction.Price.Value == default)
            {
                _logger.LogWarning("PredictedPrice is not defined for Predictor={Predictor}", prediction.Predictor);
                continue;
            }

            if (data.MarketQuote == null)
            {
                _logger.LogWarning("MarketQuote is not defined for InstrumentId={InstrumentId}", prediction.InstrumentId);
                continue;
            }

            var direction = GetTradingDirection(prediction.Price.Value, data.MarketQuote.Value, data.PriceThreshold);

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

            data.TradingSignals.Add(signal);
        }

        _logger.LogInformation("#{Sequence} TradingSignalsGenerator executed.", sequence);
    }

    // TODO: Test it
    internal static TradingDirection GetTradingDirection(decimal predictedPrice, Quote marketQuote, double threshold)
    {
        // цена будет выше минимальной цены предложения
        var askDelta = (double)((predictedPrice - marketQuote.Ask) / marketQuote.Ask);
        if (askDelta >= threshold)
        {

            return TradingDirection.Buy;
        }

        // цена будет ниже максимальной цены спроса
        var bidDelta = (double)((marketQuote.Bid - predictedPrice) / predictedPrice);
        return bidDelta >= threshold ? TradingDirection.Sell : TradingDirection.Hold;
    }
}
