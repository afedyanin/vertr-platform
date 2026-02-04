namespace Vertr.Common.Application.Abstractions;

public interface IEventProcessingPipeline<T> where T : IMarketDataEvent
{
    public Task Start(CancellationToken cancellationToken = default);

    public Task Handle(T tEvent);
}
