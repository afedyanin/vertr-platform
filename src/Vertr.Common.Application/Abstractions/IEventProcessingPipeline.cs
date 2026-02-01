namespace Vertr.Common.Application.Abstractions;

public interface IEventProcessingPipeline<T>
{
    public Task Start(CancellationToken cancellationToken = default);

    public Task Stop();

    public void Handle(T tEvent);

    // optional external handler
    public Func<T, ValueTask>? OnEventReceived { get; set; }
}
