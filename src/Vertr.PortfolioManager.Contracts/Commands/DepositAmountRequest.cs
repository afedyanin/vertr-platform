using Vertr.Platform.Common.Mediator;

namespace Vertr.PortfolioManager.Contracts.Commands;

public class DepositAmountRequest : IRequest
{
    public required string AccountId { get; init; }

    public Guid PortfolioId { get; init; }

    public required Money Amount { get; init; }

    public DateTime CreatedAt { get; init; }
}
