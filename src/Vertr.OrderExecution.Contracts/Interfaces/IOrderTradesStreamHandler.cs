using Vertr.TinvestGateway.Contracts;

namespace Vertr.OrderExecution.Contracts.Interfaces;

public interface IOrderTradesStreamHandler
{
    public Task Handle(OrderTrades? response, CancellationToken token);
}
