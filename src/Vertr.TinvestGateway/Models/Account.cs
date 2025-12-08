namespace Vertr.TinvestGateway.Models;

public record class Account(
    string Id,
    string Name,
    string AccessLevel,
    string Status,
    string AccountType,
    DateTime OpenedDate,
    DateTime? ClosedDate);