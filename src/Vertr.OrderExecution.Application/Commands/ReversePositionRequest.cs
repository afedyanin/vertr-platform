using MediatR;

namespace Vertr.OrderExecution.Application.Commands;
public class ReversePositionRequest : IRequest<ReversePositionResponse>
{
    public Guid RequestId { get; init; }

    public Guid InstrumentId { get; init; }

    public required string AccountId { get; init; }
}
