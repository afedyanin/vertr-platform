using MediatR;

namespace Vertr.PortfolioManager.Contracts.Commands;

public class PayInCommand : IRequest
{
    public required string AccountId { get; init; }

    public Guid SubAccountId { get; init; }

    public required Money Amount { get; init; }

    public DateTime CreatedAt { get; init; }
}
