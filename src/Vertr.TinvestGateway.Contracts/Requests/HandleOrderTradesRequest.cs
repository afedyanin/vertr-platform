using MediatR;

namespace Vertr.TinvestGateway.Contracts.Requests;

public class HandleOrderTradesRequest : IRequest
{
    public OrderTrades? OrderTrades { get; init; }
}
