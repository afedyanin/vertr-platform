using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vertr.Common.Application;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.Services;

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
            .SetMinimumLevel(LogLevel.Warning)
        );

        var serviceProvider = services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(nameof(RunBacktest));
        var portfolioRepo = serviceProvider.GetRequiredService<IPortfoliosLocalStorage>();

        logger.LogInformation($"Init Random Walk portfolio...");
        portfolioRepo.Init(["RandomWalk"]);
        await RunBacktest(serviceProvider, logger, steps: 10);
        logger.LogInformation("Execution completed.");

        await Task.Delay(1000);
    }

    public static async Task RunBacktest(
        IServiceProvider serviceProvider,
        ILogger logger,
        int steps = 10)
    {
        logger.LogInformation("Starting backtest Steps={Steps}.", steps);

        // Create Backtest candles
        var candles = RandomCandleGenerator.GetRandomCandles(
            SberId,
            DateTime.UtcNow.AddHours(-30),
            100.0m,
            TimeSpan.FromMinutes(1),
            steps);

        var pipeline = serviceProvider.GetRequiredService<ICandleProcessingPipeline>();

        await pipeline.OnBeforeStart();

        foreach (var candle in candles)
        {
            pipeline.Handle(candle);
        }

        await pipeline.OnBeforeStop(true);

        logger.LogInformation("Backtest completed.");
    }
}
