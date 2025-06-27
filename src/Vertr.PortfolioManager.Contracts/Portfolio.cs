namespace Vertr.PortfolioManager.Contracts;
public class Portfolio
{
    public required PortfolioIdentity Identity { get; set; }

    public DateTime UpdatedAt { get; set; }

    public IList<Position> Positions { get; set; } = [];
}
