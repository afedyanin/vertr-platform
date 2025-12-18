using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vertr.Common.Application;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.Extensions;
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
        services.AddBacktestGateway();

        services.AddLogging(builder => builder
            .AddConsole()
            .SetMinimumLevel(LogLevel.Warning)
        );

        var serviceProvider = services.BuildServiceProvider();

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(nameof(RunBacktest));

        var tradingGateway = serviceProvider.GetRequiredService<ITradingGateway>();
        var instruments = await tradingGateway.GetAllInstruments();
        var portfolioRepo = serviceProvider.GetRequiredService<IPortfoliosLocalStorage>();

        logger.LogInformation($"Init Random Walk portfolio...");
        portfolioRepo.Init(["RandomWalk"]);

        await RunBacktest(serviceProvider, instruments, logger, steps: 1000);

        var portfolioManager = serviceProvider.GetRequiredService<IPortfolioManager>();
        await portfolioManager.CloseAllPositions();

        logger.LogWarning(DumpPortfolios(portfolioRepo, instruments));
        logger.LogInformation("Execution completed.");

        await Task.Delay(100);
    }

    public static async Task RunBacktest(
        IServiceProvider serviceProvider,
        Instrument[] instruments,
        ILogger logger,
        int steps = 10)
    {
        logger.LogInformation("Starting backtest Steps={Steps}.", steps);

        var candleRepository = serviceProvider.GetRequiredService<ICandlesLocalStorage>();
        var tradingGateway = serviceProvider.GetRequiredService<ITradingGateway>();
        var portfolioRepo = serviceProvider.GetRequiredService<IPortfoliosLocalStorage>();
        var pipeline = ApplicationRegistrar.CreateCandleReceivedPipeline(serviceProvider);

        await LoadHistoricCandles(SberId, candleRepository, tradingGateway);

        // Create Backtest candles
        var candles = RandomCandleGenerator.GetRandomCandles(
            SberId,
            DateTime.UtcNow.AddHours(-30),
            100.0m,
            TimeSpan.FromMinutes(1),
            steps);

        var historicCandles = candleRepository.Get(SberId);
        Debug.Assert(historicCandles.Last().TimeUtc < candles.First().TimeUtc, "Candles time mismatch.");

        var sber = instruments.Single(x => x.Id == SberId);
        var seqNum = 0;

        foreach (var candle in candles)
        {
            seqNum++;
            logger.LogInformation($"#{seqNum} Start processing candle. Time={candle.TimeUtc:s}");
            try
            {
                candleRepository.Update(candle);

                var evt = new CandleReceivedEvent
                {
                    Sequence = seqNum,
                    Candle = candle,
                    Instrument = sber,
                };

                foreach (var handler in pipeline)
                {
                    await handler.OnEvent(evt);
                }

                //logger.LogWarning(evt.Dump());
                //logger.LogWarning(DumpPortfolios(portfolioRepo, instruments));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "#{Seq} Error. Message={Message}", seqNum, ex.Message);
            }
        }

        Debug.Assert(seqNum == steps);
        logger.LogInformation("Backtest completed.");
    }

    private static async Task LoadHistoricCandles(Guid instrumentId, ICandlesLocalStorage candleRepository, ITradingGateway tradingGateway)
    {
        if (candleRepository.GetCount(instrumentId) >= candleRepository.CandlesBufferLength)
        {
            return;
        }

        var historicCandles = await tradingGateway.GetCandles(instrumentId, candleRepository.CandlesBufferLength);
        candleRepository.Load(historicCandles);
    }

    private static string DumpPortfolios(
        IPortfoliosLocalStorage portfoliosLocalStorage,
        Instrument[] instruments)
    {
        var portfolios = portfoliosLocalStorage.GetAll();

        var sb = new StringBuilder();

        foreach (var kvp in portfolios)
        {
            sb.AppendLine(kvp.Value.Dump(kvp.Key, instruments));
        }

        return sb.ToString();
    }
}
