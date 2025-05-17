using MediatR;
using Vertr.OrderExecution.Contracts;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Application.Commands;
public class PostOrderRequest : IRequest<OrderExecutionResponse>
{
    public Guid RequestId { get; init; }

    public Guid InstrumentId { get; init; }

    public long QtyLots { get; init; }

    public required PortfolioIdentity PortfolioId { get; init; }
}
