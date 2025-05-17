using Vertr.OrderExecution.Application.Abstractions;
using Vertr.OrderExecution.Contracts;

namespace Vertr.OrderExecution.Services;

public class KafkaOperationsPublisher : IOperationsPublisher
{
    public Task Publish(
        OrderOperation[] operations,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
