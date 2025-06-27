namespace Vertr.PortfolioManager.Contracts;
public class PortfolioSnapshot
{
    public required PortfolioIdentity Identity { get; set; }

    public DateTime UpdatedAt { get; set; }

    public IList<PortfolioPosition> Positions { get; set; } = [];
}
