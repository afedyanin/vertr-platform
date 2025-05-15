using MediatR;

namespace Vertr.OrderExecution.Application.Commands;
public class OpenPositionRequest : IRequest<OpenPositionResponse>
{
    public Guid RequestId { get; init; }

    public Guid InstrumentId { get; init; }

    public required string AccountId { get; init; }

    public long QtyLots { get; init; }
}
