namespace Vertr.Contracts;
public record class PositionInstrument
{
    public Guid InstrumentId { get; init; }

    public Guid PositionId { get; init; }

    public long Balance { get; init; }

    public long Blocked { get; init; }
}
