namespace Vertr.PortfolioManager.Contracts;
public record class CreatePortfolioRequest(
    Guid PortfolioId,
    string AccountId,
    PortfolioType PortfolioType,
    string Name,
    string Description
    );
