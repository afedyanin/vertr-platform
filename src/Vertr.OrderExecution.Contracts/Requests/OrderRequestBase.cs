using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Contracts.Requests;

public abstract class OrderRequestBase
{
    public Guid RequestId { get; init; }

    public Guid InstrumentId { get; init; }

    public required PortfolioIdentity PortfolioIdentity { get; init; }
}
