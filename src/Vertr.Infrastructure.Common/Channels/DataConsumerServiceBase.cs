using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vertr.Platform.Common.Channels;

namespace Vertr.Infrastructure.Common.Channels;
public abstract class DataConsumerServiceBase<T> : BackgroundService where T : class
{
    protected IServiceProvider ServiceProvider { get; private set; }

    protected DataConsumerServiceBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = ServiceProvider.GetRequiredService<IDataConsumer<T>>();
        return consumer.Consume(Handle, stoppingToken);
    }

    protected abstract Task Handle(T data, CancellationToken cancellationToken = default);
}
