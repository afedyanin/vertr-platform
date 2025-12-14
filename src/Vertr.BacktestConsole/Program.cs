using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vertr.Common.Application;

namespace Vertr.BacktestConsole;

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-10.0
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-10.0#ihostapplicationlifetime

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IConnectionMultiplexer>((sp) =>
                ConnectionMultiplexer.Connect("localhost"));

                services.AddApplication();

                services.AddSingleton(provider =>
                provider.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("TradingConsole"));
            })
            .Build();

        var t1 = MaketDataGenerator();
        var t2 = host.RunAsync();

        await Task.WhenAny(t1, t2);
    }

    public static async Task MaketDataGenerator()
    {
        for (var i = 0; i <= 10; i++)
        {
            Console.WriteLine($"Iteration #{i}");
            await Task.Delay(1000);
        }
    }
}
