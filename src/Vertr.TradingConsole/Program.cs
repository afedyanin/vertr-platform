using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vertr.Common.Application;
using Vertr.TradingConsole.BackgroundServices;

namespace Vertr.TradingConsole;

internal sealed class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
             .ConfigureServices((hostContext, services) =>
             {

                 services.AddSingleton<IConnectionMultiplexer>((sp) => ConnectionMultiplexer.Connect("localhost"));

                 services.AddApplication();
                 services.AddTinvestGateway("http://localhost:5099");

                 // Background Services
                 services.AddHostedService<MarketCandlesSubscriber>();
                 services.AddHostedService<PortfolioSubscriber>();

                 services.AddLogging(configure =>
                 {
                     configure.AddConsole(); // Add console logging
                     configure.SetMinimumLevel(LogLevel.Debug); // Set minimum log level
                 });

                 services.AddSingleton(provider => provider.GetRequiredService<ILoggerFactory>().CreateLogger("TradingConsole"));
             })
             .Build();

        await host.RunAsync();
    }
}