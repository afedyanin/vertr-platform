namespace Vertr.PortfolioManager.Contracts;
public class Portfolio
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsBacktest { get; set; }

    public IList<Position> Positions { get; set; } = [];
}
