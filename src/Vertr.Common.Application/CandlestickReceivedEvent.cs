using Vertr.Common.Contracts;

namespace Vertr.Common.Application;

public record class CandlestickReceivedEvent
{
    public required Candle Candle { get; set; }

    public List<Prediction> Predictions { get; } = [];

    public List<TradingSignal> TradingSignals { get; } = [];

    public List<OrderRequest> OrderRequests { get; } = [];
}
