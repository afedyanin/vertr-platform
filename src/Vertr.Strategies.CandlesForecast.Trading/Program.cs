using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using Vertr.Clients.TinvestApiClient;
using Vertr.Common.Application;
using Vertr.Common.Contracts;
using Vertr.Common.ForecastClient;
using Vertr.Strategies.CandlesForecast.Trading.BackgroundServices;

namespace Vertr.Strategies.CandlesForecast.Trading;

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

        var tinvestGatewayUrl = configuration.GetValue<string>("TinvestGateway:BaseAddress");
        Debug.Assert(!string.IsNullOrEmpty(tinvestGatewayUrl));
        builder.Services.AddTinvestGateway(tinvestGatewayUrl);



        var forecastGatewayUrl = configuration.GetValue<string>("VertrForecastGateway:BaseAddress");
        Debug.Assert(!string.IsNullOrEmpty(forecastGatewayUrl));
        builder.Services.AddVertrForecastClient(forecastGatewayUrl);

        builder.Services
            .AddOptionsWithValidateOnStart<ThresholdSettings>()
            .Bind(configuration.GetSection(nameof(ThresholdSettings)));

        builder.Services.AddApplication();
        builder.Services.AddOrderBookQuoteProvider();

        builder.Services.AddHostedService<MarketCandlesSubscriber>();
        builder.Services.AddHostedService<MarketOrderBookSubscriber>();
        builder.Services.AddHostedService<PortfolioSubscriber>();
        builder.Services.AddHostedService<OrderBookWatcher>();

        var host = builder.Build();
        await host.RunAsync();
    }
}