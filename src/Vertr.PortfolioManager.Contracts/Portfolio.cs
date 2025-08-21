namespace Vertr.PortfolioManager.Contracts;
public class Portfolio
{
    public Guid Id { get; set; }

    public DateTime UpdatedAt { get; set; }

    public IList<Position> Positions { get; set; } = [];
}
