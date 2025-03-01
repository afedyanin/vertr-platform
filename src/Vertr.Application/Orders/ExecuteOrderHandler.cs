using MediatR;

namespace Vertr.Application.Orders;
internal class ExecuteOrderHandler : IRequestHandler<ExecuteOrderRequest>
{
    public Task Handle(ExecuteOrderRequest request, CancellationToken cancellationToken)
    {
        // post order to gateway
        // get resposne
        // save order response into db with
        // - accountId
        // - signalId
        throw new NotImplementedException();
    }
}
