using Vertr.Platform.Common;

namespace Vertr.PortfolioManager.Contracts.Commands;

public class PayInCommand : ICommand
{
    public required string AccountId { get; init; }

    public Guid SubAccountId { get; init; }

    public required Money Amount { get; init; }
}
