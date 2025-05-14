namespace Vertr.PortfolioManager.Application.Entities;
public class PortfolioSnapshot
{
    public Guid Id { get; set; }

    public required string AccountId { get; set; }

    public DateTime UpdatedAt { get; set; }

    public PortfolioPosition[] Positions { get; set; } = [];

    public string? JsonData { get; set; }

    public string? JsonDataType { get; set; }
}
