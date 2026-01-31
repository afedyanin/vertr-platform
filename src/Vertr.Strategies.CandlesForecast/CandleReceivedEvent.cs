using Vertr.Common.Contracts;
using Vertr.Strategies.CandlesForecast.Models;

namespace Vertr.Strategies.CandlesForecast;

public record class CandleReceivedEvent
{
    public long Sequence { get; set; }

    public required Candle Candle { get; set; }

    public required Instrument Instrument { get; set; }

    public PredictionSampleInfo PredictionSampleInfo { get; set; }

    public Quote? MarketQuote { get; set; }

    public double PriceThreshold { get; set; }

    public List<Prediction> Predictions { get; } = [];

    public List<TradingSignal> TradingSignals { get; } = [];

    public List<MarketOrderRequest> OrderRequests { get; } = [];
}
