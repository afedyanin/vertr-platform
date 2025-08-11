using Microsoft.Extensions.DependencyInjection;
using Vertr.Platform.Common.Channels;

namespace Vertr.Infrastructure.Common.Channels;

public static class DataChannelRegistrar
{
    public static IServiceCollection RegisterDataChannel<T>(
        this IServiceCollection services) where T : class
    {
        services.AddSingleton<DataChannel<T>>();
        services.AddSingleton<IDataProducer<T>>(x => x.GetRequiredService<DataChannel<T>>());
        services.AddSingleton<IDataConsumer<T>>(x => x.GetRequiredService<DataChannel<T>>());

        return services;
    }
}
