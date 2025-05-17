namespace Vertr.OrderExecution.Application.Abstractions;
public interface IOperationsPublisher
{
    public Task Publish(CancellationToken cancellationToken);
}
