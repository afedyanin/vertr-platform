using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.EventHandlers;

internal sealed class TradingSignalsGenerator : IEventHandler<CandleReceivedEvent>
{
    private readonly IMarketQuoteProvider _marketQuoteProvider;
    private readonly ILogger<TradingSignalsGenerator> _logger;
    private readonly ThresholdSettings _thresholdSettings;

    public TradingSignalsGenerator(
        ILogger<TradingSignalsGenerator> logger,
        IMarketQuoteProvider marketQuoteProvider,
        IOptions<ThresholdSettings> thresholdOptions)
    {
        _logger = logger;
        _marketQuoteProvider = marketQuoteProvider;
        _thresholdSettings = thresholdOptions.Value;
    }

    public ValueTask OnEvent(CandleReceivedEvent data)
    {
        foreach (var prediction in data.Predictions)
        {
            if (prediction.Value == null || prediction.Value.Value == default)
            {
                _logger.LogWarning("PredictedPrice is not defined for Predictor={Predictor}", prediction.Predictor);
                continue;
            }

            data.MarketQuote = _marketQuoteProvider.GetMarketQuote(data.Instrument!.Id);

            if (data.MarketQuote == null)
            {
                _logger.LogWarning("MarketQuote is not defined for InstrumentId={InstrumentId}", prediction.InstrumentId);
                continue;
            }

            if (_thresholdSettings.UseStatsThreshold)
            {
                var stats = data.PredictionSampleInfo.ClosePriceStats;
                data.PriceThreshold = stats.StdDev / stats.Mean * _thresholdSettings.ThresholdSigma;
            }
            else
            {
                data.PriceThreshold = _thresholdSettings.ThresholdValue * _thresholdSettings.ThresholdSigma;
            }

            var direction = GetTradingDirection(prediction.Value.Value, data.MarketQuote.Value, data.PriceThreshold);

            if (direction == TradingDirection.Hold)
            {
                continue;
            }

            var signal = new TradingSignal
            {
                Predictor = prediction.Predictor,
                Instrument = data.Instrument,
                Direction = direction,
            };

            data.TradingSignals.Add(signal);
        }

        _logger.LogDebug("#{Sequence} TradingSignalsGenerator executed. {SignalsCount} signals added.", data.Sequence, data.TradingSignals.Count);

        return ValueTask.CompletedTask;
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
