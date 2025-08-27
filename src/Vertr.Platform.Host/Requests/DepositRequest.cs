namespace Vertr.Platform.Host.Requests;

public record class DepositRequest(DateTime Date, Guid PortfolioId, decimal Amount, string Currency);
