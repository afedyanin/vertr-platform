namespace Vertr.Contracts.Messages;

public record class PositionData
{
    public required string AccountId { get; init; }

    public DateTime Timestamp { get; init; }

    public MoneyValue[] Money { get; init; } = [];

    public PositionInstrument[] Securities { get; init; } = [];

    public PositionInstrument[] Futures { get; init; } = [];

    public PositionInstrument[] Options { get; init; } = [];
}

