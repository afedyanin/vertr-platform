using MediatR;

namespace Vertr.OrderExecution.Contracts.Requests;

public class OrderTradesRequest : IRequest
{
    public Guid InstrumentId { get; init; }

    public required OrderTrades OrderTrades { get; init; }
}
