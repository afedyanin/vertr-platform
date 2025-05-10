namespace Vertr.Portfolio.Domain;

public class Position
{
    public Guid PositionId { get; init; }

    public Guid PortfolioId { get; init; }

    public required string Ticker { get; init; }

    public long Balance { get; init; }
}
