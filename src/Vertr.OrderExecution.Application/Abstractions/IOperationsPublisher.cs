using Vertr.OrderExecution.Contracts;

namespace Vertr.OrderExecution.Application.Abstractions;
public interface IOperationsPublisher
{
    public Task Publish(
        OrderOperation[] operations,
        CancellationToken cancellationToken = default);
}
