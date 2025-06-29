using MediatR;

namespace Vertr.PortfolioManager.Contracts.Requests;
public class PayInOperationRequest : IRequest
{
    public required string AccountId { get; init; }

    public Guid SubAccountId { get; init; }

    public required Money Amount { get; init; }
}
