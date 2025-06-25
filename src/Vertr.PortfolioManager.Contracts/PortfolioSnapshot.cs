namespace Vertr.PortfolioManager.Contracts;
public class PortfolioSnapshot
{
    public required string AccountId { get; set; }

    public Guid BookId { get; set; }

    public DateTime UpdatedAt { get; set; }

    public IList<PortfolioPosition> Positions { get; set; } = [];
}
