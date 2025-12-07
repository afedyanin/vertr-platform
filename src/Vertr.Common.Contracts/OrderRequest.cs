namespace Vertr.Common.Contracts;

public record class OrderRequest
{
    public required Guid InstrumentId { get; init; }
    public required Guid RequestId { get; init; }
    public required Guid PortfolioId { get; init; }
    public required OrderDirection OrderDirection { get; init; }
    public required decimal Price { get; init; }
    public required long QuantityLots { get; init; }
}
public enum OrderDirection
{
    Unspecified = 0,
    Buy = 1,
    Sell = 2,
}

