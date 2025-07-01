using MediatR;

namespace Vertr.PortfolioManager.Contracts.Requests;

public record class PositionOverride(Guid InstrumentId, decimal Balance);

public class PositionOverridesRequest : IRequest
{
    public required string AccountId { get; init; }

    public Guid SubAccountId { get; init; }

    public PositionOverride[] Overrides { get; init; } = [];
}
