using MediatR;

namespace Vertr.TinvestGateway.Contracts.Requests;

public class HandleOrderStateRequest : IRequest
{
    public OrderState? OrderState { get; init; }

    public string? AccountId { get; init; }
}
