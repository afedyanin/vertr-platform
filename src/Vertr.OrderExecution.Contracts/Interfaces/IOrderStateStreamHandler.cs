using Vertr.TinvestGateway.Contracts;

namespace Vertr.OrderExecution.Contracts.Interfaces;

public interface IOrderStateStreamHandler
{
    public Task Handle(OrderState? response, CancellationToken token);
}
