namespace Vertr.PortfolioManager.Contracts;
public record class PortfolioSnapshot(
    string AccountId,
    DateTime UpdatedAt,
    PortfolioPosition[] Positions
    );
