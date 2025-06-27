namespace Vertr.PortfolioManager.Contracts;

public record class PortfolioIdentity(string AccountId, Guid? BookId = null);
