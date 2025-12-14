using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Vertr.Common.DataAccess.Repositories;

namespace Vertr.Common.DataAccess.Redis;

public static class RedisDataAccessRegistrar
{
    public static IServiceCollection AddCommonRedisDataAccess(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ICandlestickRepository, CandlestickRepository>();
        services.AddTransient<IInstrumentRepository, InstrumentRepository>();
        services.AddTransient<IPortfolioRepository, PortfolioRepository>();
        services.AddTransient<IOrderBookRepository, OrderBookRepository>();

        var redisConnectionString = configuration.GetConnectionString("RedisConnection");

        Debug.Assert(!string.IsNullOrEmpty(redisConnectionString));

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConnectionString!));

        return services;
    }
}