namespace Vertr.PortfolioManager.Application.Entities;

public class PortfolioMetadata
{
    public Guid PortfolioId { get; set; }

    public required string AccountId { get; set; }

    public PortfolioType PortfolioType { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }
}
