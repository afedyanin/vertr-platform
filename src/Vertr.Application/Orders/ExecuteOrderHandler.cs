using MediatR;
using Vertr.Domain.Ports;

namespace Vertr.Application.Orders;
internal class ExecuteOrderHandler : IRequestHandler<ExecuteOrderRequest>
{
    private readonly ITinvestGateway _gateway;

    public ExecuteOrderHandler(ITinvestGateway gateway)
    {
        _gateway = gateway;
    }

    public async Task Handle(ExecuteOrderRequest request, CancellationToken cancellationToken)
    {
        var response = await _gateway.PostOrder(request.PostOrderRequest);

        // save order response into db with
        // - accountId
        // - signalId
        throw new NotImplementedException();
    }
}
