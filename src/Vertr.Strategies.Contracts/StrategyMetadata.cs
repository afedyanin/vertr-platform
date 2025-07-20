namespace Vertr.Strategies.Contracts;

public class StrategyMetadata
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public required string Type { get; set; }
}
