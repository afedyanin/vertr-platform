using Vertr.Common.Contracts;

namespace Vertr.Common.Application;

public record class CandlestickReceivedEvent
{
    public Candlestick Candlestick { get; set; }

    public Prediction[] Predictions { get; set; } = [];

    public TradingSignal[] TradingSignal { get; set; } = [];

    public OrderRequest[] OrderRequests { get; set; } = [];
}
