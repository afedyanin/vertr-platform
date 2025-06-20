using MediatR;

namespace Vertr.OrderExecution.Contracts.Requests;

public class ClosePositionRequest : IRequest<OrderExecutionResponse>
{
    public Guid RequestId { get; init; }

    public Guid InstrumentId { get; init; }

    public required PortfolioIdentity PortfolioId { get; init; }
}
