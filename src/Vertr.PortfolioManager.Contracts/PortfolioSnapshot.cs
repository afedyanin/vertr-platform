namespace Vertr.PortfolioManager.Contracts;
public record class PortfolioSnapshot(
    Guid? PortfolioId,
    string AccountId,
    DateTime UpdatedAt,
    PortfolioPosition[] Positions
    );
