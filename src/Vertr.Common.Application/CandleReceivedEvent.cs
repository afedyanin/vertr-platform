using Vertr.Common.Contracts;

namespace Vertr.Common.Application;

public record class CandleReceivedEvent
{
    public Candle? Candle { get; set; }

    public Instrument? Instrument { get; set; }

    public PredictionSampleInfo PredictionSampleInfo { get; set; }

    public Quote? MarketQuote { get; set; }

    public double PriceThreshold { get; set; }

    public List<Prediction> Predictions { get; } = [];

    public List<TradingSignal> TradingSignals { get; } = [];

    public List<MarketOrderRequest> OrderRequests { get; } = [];
}
