using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.Extensions;

namespace Vertr.BacktestConsole;

internal static class Program
{
    private static readonly Guid SberId = new Guid("e6123145-9665-43e0-8413-cd61b8aa9b13");

    public static async Task Main(string[] args)
    {
        var services = new ServiceCollection();

        services.AddApplication();
        services.AddBacktest();

        services.AddLogging(builder => builder
            .AddConsole()
            .SetMinimumLevel(LogLevel.Information)
        );

        var serviceProvider = services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(nameof(Program));

        var portfolioRepo = serviceProvider.GetRequiredService<IPortfoliosLocalStorage>();
        var historicCandlesProvider = serviceProvider.GetRequiredService<IHistoricCandlesProvider>();
        var pipeline = serviceProvider.GetRequiredService<ICandleProcessingPipeline>();

        logger.LogInformation($"Init Random Walk portfolio...");
        portfolioRepo.Init(["RandomWalk"]);

        var steps = 10;
        await historicCandlesProvider.Load("Data\\SBER_251101_251109.csv", SberId);
        var candles = historicCandlesProvider.Get(SberId, skip: 100, take: steps);
        logger.LogWarning($"Init historic candles. {candles.GetCandlesRange()}");

        logger.LogInformation("Starting backtest...");
        await pipeline.Start(dumpPortfolios: true);

        foreach (var candle in candles)
        {
            pipeline.Handle(candle);
        }

        await pipeline.Stop();
        logger.LogInformation("Backtest completed.");

        await Task.Delay(1000);
    }
}
