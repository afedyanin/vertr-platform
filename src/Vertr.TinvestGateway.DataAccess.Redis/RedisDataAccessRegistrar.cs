using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.DataAccess.Redis;

public static class RedisDataAccessRegistrar
{
    public static IServiceCollection AddRedisDataAccess(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ICandlestickRepository, CandlestickRepository>();
        services.AddTransient<IInstrumentRepository, InstrumentRepository>();
        services.AddTransient<IOrderRequestRepository, OrderRequestRepository>();
        services.AddTransient<IOrderResponseRepository, OrderResponseRepository>();
        services.AddTransient<IOrderStateRepository, OrderStateRepository>();
        services.AddTransient<IOrderTradeRepository, OrderTradeRepository>();
        services.AddTransient<IPortfolioRepository, PortfolioRepository>();

        var redisConnectionString = configuration.GetConnectionString("RedisConnection");

        Debug.Assert(!string.IsNullOrEmpty(redisConnectionString));

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConnectionString!));

        return services;
    }
}