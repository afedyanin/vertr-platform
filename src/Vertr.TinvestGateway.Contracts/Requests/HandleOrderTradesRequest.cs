using MediatR;

namespace Vertr.TinvestGateway.Contracts.Requests;

// TODO: move to Order Engine

public class HandleOrderTradesRequest : IRequest
{
    public OrderTrades? OrderTrades { get; init; }
}
