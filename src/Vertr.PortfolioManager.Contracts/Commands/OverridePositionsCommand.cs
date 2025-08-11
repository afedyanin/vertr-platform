using MediatR;

namespace Vertr.PortfolioManager.Contracts.Commands;

public record class PositionOverride(Guid InstrumentId, decimal Balance);

public class OverridePositionsCommand : IRequest
{
    public required string AccountId { get; init; }

    public Guid SubAccountId { get; init; }

    public PositionOverride[] Overrides { get; init; } = [];

    public DateTime CreatedAt { get; init; }
}
