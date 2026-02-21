namespace Vertr.Common.Contracts;

public record class TradeDetails
{
    public int Sequence { get; init; }
    public decimal MaxBid { get; init; }
    public decimal MinAsk { get; init; }
    public TradingDirection OrderBookDirection { get; init; }
    public decimal FairPrice { get; init; }

    // добавить цены из стакана фьюча
    public required Guid InstrumentId { get; init; }
    public required Guid RequestId { get; init; }
    public required Guid PortfolioId { get; init; }
    public required long QuantityLots { get; init; }
    public TradingDirection RequestDirection { get; init; }
    public string? OrderId { get; init; }
    public string? TradeId { get; init; }
    public DateTime ExecutionTime { get; init; }
    public decimal Price { get; init; }
    public long Quantity { get; init; }
}
