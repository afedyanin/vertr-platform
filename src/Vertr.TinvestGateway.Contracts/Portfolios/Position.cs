namespace Vertr.TinvestGateway.Contracts.Portfolios;

public record struct Position
{
    public Guid InstrumentId { get; set; }

    public decimal Balance { get; set; }
}