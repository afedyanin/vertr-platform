namespace Vertr.Common.Contracts;

public record class MarketOrderRequest
{
    public required Guid InstrumentId { get; init; }
    public required Guid RequestId { get; init; }
    public required Guid PortfolioId { get; init; }
    public required long QuantityLots { get; init; }
}

