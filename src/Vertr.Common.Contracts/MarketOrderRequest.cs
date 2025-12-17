namespace Vertr.Common.Contracts;

public record class MarketOrderRequest
{
    public required string Predictor { get; set; }
    public required Guid InstrumentId { get; init; }
    public required Guid RequestId { get; init; }
    public required Guid PortfolioId { get; init; }
    public required long QuantityLots { get; init; }

    public TradingDirection Direction { get; init; }
}

