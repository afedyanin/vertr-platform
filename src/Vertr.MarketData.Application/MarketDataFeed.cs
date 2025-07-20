using System.Threading.Channels;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.MarketData.Application;

internal class MarketDataFeed : IMarketDataPublisher, IMarketDataConsumer
{
    private readonly Channel<Candle> _channel;

    public MarketDataFeed()
    {
        _channel = Channel.CreateUnbounded<Candle>();
    }

    public async Task Publish(Candle candle, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _channel.Writer.Complete();
            return;
        }

        await _channel.Writer.WriteAsync(candle, cancellationToken);
    }

    public async Task Consume(
        Func<Candle, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default)
    {
        while (await _channel.Reader.WaitToReadAsync(cancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            var candle = await _channel.Reader.ReadAsync(cancellationToken);
            await handler(candle, cancellationToken);
        }
    }
}
