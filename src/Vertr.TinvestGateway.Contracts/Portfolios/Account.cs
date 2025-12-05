namespace Vertr.TinvestGateway.Contracts.Portfolios;

public record class Account(
    string Id,
    string Name,
    string AccessLevel,
    string Status,
    string AccountType,
    DateTime OpenedDate,
    DateTime? ClosedDate);