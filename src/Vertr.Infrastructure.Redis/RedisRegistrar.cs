using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Vertr.Infrastructure.Redis;

public static class RedisRegistrar
{
    public static IServiceCollection AddRedis(this IServiceCollection services, ConfigurationOptions configOptions)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(configOptions));

        return services;
    }

}
