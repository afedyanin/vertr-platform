using MediatR;

namespace Vertr.OrderExecution.Contracts.Requests;

public class OrderStateRequest : IRequest
{
    public OrderState? OrderState { get; init; }

    public string? AccountId { get; init; }
}
