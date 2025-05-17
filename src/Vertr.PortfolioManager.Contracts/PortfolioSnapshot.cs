namespace Vertr.PortfolioManager.Contracts;
public record class PortfolioSnapshot(
    string AccountId,
    Guid? BookId,
    DateTime UpdatedAt,
    PortfolioPosition[] Positions
    );
