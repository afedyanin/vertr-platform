using Disruptor;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Services;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.EventHandlers;

internal sealed class TradingSignalsGenerator : IEventHandler<CandlestickReceivedEvent>
{
    private readonly ILogger<TradingSignalsGenerator> _logger;
    private readonly IOrderBookRepository _orderBookRepository;

    public TradingSignalsGenerator(
        IOrderBookRepository orderBookRepository,
        ILogger<TradingSignalsGenerator> logger)
    {
        _orderBookRepository = orderBookRepository;
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
                Direction = GetTradingDirection(prediction.InstrumentId, prediction.Price)
            };

            data.TradingSignals.Add(signal);
        }

        _logger.LogInformation("TradingSignalsGenerator executed.");
    }

    private TradingDirection GetTradingDirection(Guid instrumentId, decimal? predictedPrice)
    {
        var quote = _orderBookRepository.GetById(instrumentId);

        if (quote == null)
        {
            return TradingDirection.Hold;
        }

        // Evaluate prediction & thresholds

        return TradingDirection.Hold;
    }
}
