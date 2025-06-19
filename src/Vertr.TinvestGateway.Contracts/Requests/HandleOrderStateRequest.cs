using MediatR;

namespace Vertr.TinvestGateway.Contracts.Requests;

// TODO: move to Order Engine
public class HandleOrderStateRequest : IRequest
{
    public OrderState? OrderState { get; init; }

    public string? AccountId { get; init; }
}
