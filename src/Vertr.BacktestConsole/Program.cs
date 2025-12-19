using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vertr.Common.Application;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.Services;
using Vertr.Common.Contracts;

namespace Vertr.BacktestConsole;

internal static class Program
{
    private static readonly Guid SberId = new Guid("e6123145-9665-43e0-8413-cd61b8aa9b13");

    public static async Task Main(string[] args)
    {
        var services = new ServiceCollection();

        services.AddSingleton<IConnectionMultiplexer>((sp) =>
        ConnectionMultiplexer.Connect("localhost"));

        services.AddApplication();
        services.AddBacktest();

        services.AddLogging(builder => builder
            .AddConsole()
            .SetMinimumLevel(LogLevel.Information)
        );

        var serviceProvider = services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(nameof(RunBacktest));
        var portfolioRepo = serviceProvider.GetRequiredService<IPortfoliosLocalStorage>();

        logger.LogInformation($"Init Random Walk portfolio...");
        portfolioRepo.Init(["RandomWalk"]);

        var steps = 10000;

        var candles = RandomCandleGenerator.GetRandomCandles(
            SberId,
            DateTime.UtcNow.AddHours(-30),
            100.0m,
            TimeSpan.FromMinutes(1),
            count: steps);

        logger.LogInformation("Starting backtest Steps={Steps}.", steps);
        await RunBacktest(candles, serviceProvider);
        logger.LogInformation("Execution completed.");

        await Task.Delay(1000);
    }

    public static async Task RunBacktest(IEnumerable<Candle> candles, IServiceProvider serviceProvider)
    {
        var pipeline = serviceProvider.GetRequiredService<ICandleProcessingPipeline>();

        await pipeline.Start(dumpPortfolios: true);

        foreach (var candle in candles)
        {
            pipeline.Handle(candle);
        }

        await pipeline.Stop();
    }
}
