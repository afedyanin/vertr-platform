namespace Vertr.Strategies.Contracts;

public record class TradingSignal
{
    public Guid Id { get; init; }

    public Guid StrategyId { get; init; }

    public required string AccountId { get; init; }

    public Guid SubAccountId { get; init; }

    public Guid InstrumentId { get; init; }

    public long QtyLots { get; init; }

    public DateTime CreatedAt { get; init; }
}
