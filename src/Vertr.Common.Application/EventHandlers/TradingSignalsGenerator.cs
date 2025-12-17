using Disruptor;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.EventHandlers;

internal sealed class TradingSignalsGenerator : IEventHandler<CandleReceivedEvent>
{
    private const int ThresholdSigma = 1;
    private const double DefaultThreshold = 0.001;

    private readonly IMarketQuoteProvider? _marketQuoteProvider;
    private readonly ILogger<TradingSignalsGenerator> _logger;

    public TradingSignalsGenerator(
        ILogger<TradingSignalsGenerator> logger,
        IMarketQuoteProvider? marketQuoteProvider = null)
    {
        _logger = logger;
        _marketQuoteProvider = marketQuoteProvider;
    }

    public void OnEvent(CandleReceivedEvent data, long sequence, bool endOfBatch)
    {
        foreach (var prediction in data.Predictions)
        {
            if (prediction.Price == null || prediction.Price.Value == default)
            {
                _logger.LogWarning("PredictedPrice is not defined for Predictor={Predictor}", prediction.Predictor);
                continue;
            }

            data.MarketQuote = _marketQuoteProvider?.GetMarketQuote(data.Instrument!.Id) ?? GetMarketQuote(data.Candle);

            if (data.MarketQuote == null)
            {
                _logger.LogWarning("MarketQuote is not defined for InstrumentId={InstrumentId}", prediction.InstrumentId);
                continue;
            }

            // TODO: Use fixed or floating threshold? - move to Hyperparams
            // var stats = data.PredictionSampleInfo.ClosePriceStats;
            // data.PriceThreshold = stats.StdDev / stats.Mean * ThresholdSigma;
            data.PriceThreshold = DefaultThreshold * ThresholdSigma;

            var direction = GetTradingDirection(prediction.Price.Value, data.MarketQuote.Value, data.PriceThreshold);

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

        _logger.LogInformation("#{Sequence} TradingSignalsGenerator executed. {SignalsCount} signals added.", sequence, data.TradingSignals.Count);
    }

    private static Quote? GetMarketQuote(Candle? last)
    {
        if (last == null)
        {
            return null;
        }

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
