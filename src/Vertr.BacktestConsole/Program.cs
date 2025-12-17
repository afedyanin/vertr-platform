using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vertr.Common.Application;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.EventHandlers;
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
        services.AddBacktestGateway();

        services.AddLogging(builder => builder
            .AddConsole()
            .SetMinimumLevel(LogLevel.Debug)
        );

        var serviceProvider = services.BuildServiceProvider();

        await RunBacktest(serviceProvider);

        // TODO: Dump portfolios
    }

    public static async Task RunBacktest(IServiceProvider serviceProvider)
    {
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(nameof(RunBacktest));

        var candleRepository = serviceProvider.GetRequiredService<ICandlesLocalStorage>();
        var portfolioRepository = serviceProvider.GetRequiredService<IPortfoliosLocalStorage>();
        var step1 = serviceProvider.GetRequiredService<MarketDataPredictor>();
        var step2 = serviceProvider.GetRequiredService<TradingSignalsGenerator>();
        // var step3 = serviceProvider.GetRequiredService<PortfolioPositionHandler>();
        // var step4 = serviceProvider.GetRequiredService<OrderExecutionHandler>();

        logger.LogInformation($"Init Random Walk portfolio...");
        portfolioRepository.Init(["RandomWalk"]);

        var candles = RandomCandleGenerator.GetRandomCandles(
            SberId,
            DateTime.UtcNow,
            100.0m,
            TimeSpan.FromMinutes(5),
            10);

        var seqNum = 0;
        foreach (var candle in candles)
        {
            logger.LogInformation($"#{seqNum} Start processing candle event.Time={candle.TimeUtc}");

            candleRepository.Update(candle);

            var evt = new CandlestickReceivedEvent
            {
                Candle = candle
            };

            step1.OnEvent(evt, seqNum, true);
            step2.OnEvent(evt, seqNum, true);
            seqNum++;

            // TODO: Dump Event
        }

        var portfolioManager = serviceProvider.GetRequiredService<IPortfolioManager>();
        await portfolioManager.CloseAllPositions();

        logger.LogInformation($"Backtest completed.");
    }
}
