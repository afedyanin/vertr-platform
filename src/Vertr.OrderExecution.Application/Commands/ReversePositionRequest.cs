using MediatR;

namespace Vertr.OrderExecution.Application.Commands;
public class ReversePositionRequest : IRequest<OrderExecutionResponse>
{
    public Guid RequestId { get; init; }

    public Guid InstrumentId { get; init; }

    public required string AccountId { get; init; }

    public Guid BookId { get; init; }
}
