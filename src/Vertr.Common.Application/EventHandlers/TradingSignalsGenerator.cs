using Disruptor;
using Microsoft.Extensions.Logging;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.EventHandlers;

internal sealed class TradingSignalsGenerator : IEventHandler<CandlestickReceivedEvent>
{
    private readonly ILogger<TradingSignalsGenerator> _logger;

    public TradingSignalsGenerator(ILogger<TradingSignalsGenerator> logger)
    {
        _logger = logger;
    }

    public void OnEvent(CandlestickReceivedEvent data, long sequence, bool endOfBatch)
    {
        foreach (var prediction in data.Predictions)
        {
            var signal = new TradingSignal
            {
                Predictor = prediction.Predictor,
                InstrumentId = prediction.InstrumentId,
                Direction = GetTradingDirection(prediction.Price)
            };

            data.TradingSignals.Add(signal);
        }


        _logger.LogInformation("TradingSignalsGenerator executed.");
    }

    private TradingDirection GetTradingDirection(decimal? predicterPrice)
    {
        // Get current market price
        // Evaluate prediction & thresholds

        return TradingDirection.Hold;
    }
}
