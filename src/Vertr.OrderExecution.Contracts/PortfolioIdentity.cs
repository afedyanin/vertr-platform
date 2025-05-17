namespace Vertr.OrderExecution.Contracts;

public record class PortfolioIdentity(string AccountId, Guid? BookId = null);
