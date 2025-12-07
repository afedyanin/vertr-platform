namespace Vertr.Common.Contracts;

public record struct Position
{
    public Guid InstrumentId { get; set; }

    public decimal Amount { get; set; }
}