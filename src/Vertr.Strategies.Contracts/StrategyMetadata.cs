namespace Vertr.Strategies.Contracts;

public class StrategyMetadata
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required StrategyType Type { get; set; }

    public Guid InstrumentId { get; set; }

    public Guid PortfolioId { get; set; }

    public long QtyLots { get; set; }

    public bool IsActive { get; set; }

    public string? ParamsJson { get; set; }

    public DateTime CreatedAt { get; set; }
}
