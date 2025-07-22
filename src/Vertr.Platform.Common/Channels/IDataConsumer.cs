namespace Vertr.Platform.Common.Channels;
public interface IDataConsumer<T> where T : class
{
    public Task Consume(
        Func<T, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default);
}
