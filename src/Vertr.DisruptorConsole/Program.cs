using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Vertr.Common.Application;

namespace Vertr.DisruptorConsole;

internal sealed class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
             .ConfigureServices((hostContext, services) =>
             {

                 services.AddApplication();
                 services.AddLogging(configure =>
                 {
                     configure.AddConsole(); // Add console logging
                     configure.SetMinimumLevel(LogLevel.Debug); // Set minimum log level
                 });

                 services.AddSingleton(provider => provider.GetRequiredService<ILoggerFactory>().CreateLogger("TradingConsole"));
             })
             .Build();

        // TODO: Move it to background service
        var disruptor = ApplicationRegistrar.CreateCandlestickPipeline(host.Services);
        disruptor.Start();

        using (var scope = disruptor.PublishEvent())
        {
            var data = scope.Event();
        }

        await host.RunAsync();

        disruptor.Shutdown();
        Console.WriteLine("Completed");
    }
}
