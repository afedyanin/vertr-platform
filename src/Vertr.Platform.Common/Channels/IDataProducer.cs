namespace Vertr.Platform.Common.Channels;
public interface IDataProducer<T> where T : class
{
    public Task Produce(T item, CancellationToken cancellationToken = default);
}
