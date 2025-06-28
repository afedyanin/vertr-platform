using MediatR;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Contracts.Requests;

public class ExecuteOrderRequest : IRequest<ExecuteOrderResponse>
{
    public Guid RequestId { get; init; }

    public Guid InstrumentId { get; init; }

    public required PortfolioIdentity PortfolioIdentity { get; init; }

    public long QtyLots { get; init; }
}
