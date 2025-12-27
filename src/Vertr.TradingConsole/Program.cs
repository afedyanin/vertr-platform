using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using Vertr.Common.Application;
using Vertr.TradingConsole.BackgroundServices;

namespace Vertr.TradingConsole;

internal sealed class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        var configuration = builder.Configuration;

        var redisConnectionString = configuration.GetConnectionString("RedisConnection");
        Debug.Assert(!string.IsNullOrEmpty(redisConnectionString));
        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConnectionString));

        var baseAddress = configuration.GetValue<string>("TinvestGateway:BaseAddress");
        Debug.Assert(!string.IsNullOrEmpty(baseAddress));
        builder.Services.AddTinvestGateway(baseAddress);

        builder.Services.AddApplication();

        builder.Services.AddHostedService<MarketCandlesSubscriber>();
        builder.Services.AddHostedService<MarketOrderBookSubscriber>();
        builder.Services.AddHostedService<PortfolioSubscriber>();

        var host = builder.Build();

        await host.RunAsync();
    }
}