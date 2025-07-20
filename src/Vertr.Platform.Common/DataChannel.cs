using System.Threading.Channels;

namespace Vertr.Platform.Common;

public class DataChannel<T> : IDataProducer<T>, IDataConsumer<T> where T : class
{
    private readonly Channel<T> _channel;

    public DataChannel()
    {
        _channel = Channel.CreateUnbounded<T>();
    }

    public async Task Produce(T item, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _channel.Writer.Complete();
            return;
        }

        await _channel.Writer.WriteAsync(item, cancellationToken);
    }

    public async Task Consume(
        Func<T, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default)
    {
        while (await _channel.Reader.WaitToReadAsync(cancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            var item = await _channel.Reader.ReadAsync(cancellationToken);
            await handler(item, cancellationToken);
        }
    }
}
