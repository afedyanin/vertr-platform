using Disruptor.Dsl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vertr.Common.Application;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.BacktestConsole;

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-10.0
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-10.0#ihostapplicationlifetime

internal static class Program
{
    private static readonly Guid SberId = new Guid("e6123145-9665-43e0-8413-cd61b8aa9b13");

    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IConnectionMultiplexer>((sp) =>
                ConnectionMultiplexer.Connect("localhost"));

                services.AddApplication();
                services.AddCandlesQuoteProvider();
                services.AddBacktestGateway();

                services.AddSingleton(provider =>
                provider.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("TradingConsole"));
            })
            .Build();

        var disruptor = ApplicationRegistrar.CreateBacktestCandlestickPipeline(host.Services);

        disruptor.Start();

        var t1 = MaketDataGenerator(host.Services, disruptor);
        var t2 = host.RunAsync();

        await Task.WhenAny(t1, t2);

        disruptor.Shutdown();

        var portfolioManager = host.Services.GetRequiredService<IPortfolioManager>();
        await portfolioManager.CloseAllPositions();
        // TODO: Dump portfolios
    }

    public static async Task MaketDataGenerator(
        IServiceProvider serviceProvider,
        Disruptor<CandlestickReceivedEvent> disruptor)
    {
        var candleRepository = serviceProvider.GetRequiredService<ICandlesLocalStorage>();
        var portfolioRepository = serviceProvider.GetRequiredService<IPortfoliosLocalStorage>();

        portfolioRepository.Init(["RandomWalk"]);
        var startTime = DateTime.UtcNow;

        for (var i = 0; i <= 10; i++)
        {
            var candle = new Candle
            (
                InstrumentId: SberId,
                TimeUtc: startTime.AddMinutes(i),
                Open: 90.0m,
                Close: 95.45m,
                High: 101.14m,
                Low: 89.89m,
                Volume: 1234
            );

            candleRepository.Update(candle);

            using (var scope = disruptor.PublishEvent())
            {
                var evt = scope.Event();
                evt.Candle = candle;
            }
        }
    }
}
