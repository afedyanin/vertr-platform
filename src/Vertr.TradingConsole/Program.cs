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
                 services.AddSingleton<IConnectionMultiplexer>((sp) =>
                    ConnectionMultiplexer.Connect("localhost"));

                 services.AddTinvestGateway("http://localhost:5099");

                 services.AddApplication();

                 services.AddHostedService<MarketCandlesSubscriber>();
                 services.AddHostedService<PortfolioSubscriber>();

                 services.AddSingleton(provider =>
                    provider.GetRequiredService<ILoggerFactory>()
                        .CreateLogger("TradingConsole"));

                 services.AddLogging(configure =>
                 {
                     configure.AddConsole();
                     configure.SetMinimumLevel(LogLevel.Information);
                 });
             })
             .Build();

        await host.RunAsync();
    }
}