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
        // Get current market price
        // Evaluate prediction & thresholds
        // Generate and save trading signals

        var signal = new TradingSignal
        {
            Predictor = "RandomWalk",
            InstrumentId = data.Candle!.InstrumentId,
            Direction = GetRandomDirection()
        };

        data.TradingSignals.Add(signal);

        _logger.LogInformation("TradingSignalsGenerator executed.");
    }

    private static TradingDirection GetRandomDirection()
        => Random.Shared.Next(0, 2) > 0 ? TradingDirection.Buy : TradingDirection.Sell;

}
