namespace Vertr.Portfolio.Domain;
public class Portfolio
{
    public Guid PortfolioId { get; init; }

    public required string AccountId { get; init; }

    public MoneyValue[] Money { get; init; } = [];

    public Position[] Positions { get; init; } = [];
}
