using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.DataAccess.Redis;

public static class TinvestDataAccessRegistrar
{
    public static IServiceCollection AddTinvestRedisDataAccess(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IOrderRequestRepository, OrderRequestRepository>();
        services.AddTransient<IOrderResponseRepository, OrderResponseRepository>();
        services.AddTransient<IOrderStateRepository, OrderStateRepository>();
        services.AddTransient<IOrderTradeRepository, OrderTradeRepository>();
        services.AddTransient<IPortfolioOrdersRepository, PortfolioOrdersRepository>();

        var redisConnectionString = configuration.GetConnectionString("RedisConnection");

        Debug.Assert(!string.IsNullOrEmpty(redisConnectionString));

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConnectionString!));

        return services;
    }
}