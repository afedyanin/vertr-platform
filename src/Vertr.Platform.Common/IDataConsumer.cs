namespace Vertr.Platform.Common;
public interface IDataConsumer<T> where T : class
{
    public Task Consume(
        Func<T, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default);
}
