using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vertr.Common.Application;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.EventHandlers;
using Vertr.Common.Application.Extensions;
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
            .SetMinimumLevel(LogLevel.Warning)
        );

        var serviceProvider = services.BuildServiceProvider();

        await RunBacktest(serviceProvider);

        Console.WriteLine("Execution completed.");

        // TODO: Dump portfolios
    }

    public static async Task RunBacktest(IServiceProvider serviceProvider, int steps = 10)
    {
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(nameof(RunBacktest));

        var candleRepository = serviceProvider.GetRequiredService<ICandlesLocalStorage>();
        var portfolioRepository = serviceProvider.GetRequiredService<IPortfoliosLocalStorage>();
        var tradingGateway = serviceProvider.GetRequiredService<ITradingGateway>();

        var step1 = serviceProvider.GetRequiredService<MarketDataPredictor>();
        var step2 = serviceProvider.GetRequiredService<TradingSignalsGenerator>();
        // var step3 = serviceProvider.GetRequiredService<PortfolioPositionHandler>();
        // var step4 = serviceProvider.GetRequiredService<OrderExecutionHandler>();

        logger.LogInformation("Starting backtest Steps={Steps}.", steps);

        logger.LogInformation($"Init Random Walk portfolio...");
        portfolioRepository.Init(["RandomWalk"]);

        var instruments = await tradingGateway.GetAllInstruments();

        // Load Historic candles
        await LoadCandles(SberId, candleRepository, tradingGateway);

        // Create Backtest candles
        var candles = RandomCandleGenerator.GetRandomCandles(
            SberId,
            DateTime.UtcNow.AddHours(-3),
            100.0m,
            TimeSpan.FromMinutes(1),
            steps);

        var historicCandles = candleRepository.Get(SberId);
        Debug.Assert(historicCandles.Last().TimeUtc < candles.First().TimeUtc, "Candles time mismatch.");

        var instrument = instruments.FirstOrDefault(x => x.Id == SberId);

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
                    Candle = candle,
                    Instrument = instrument,
                };
                step1.OnEvent(evt, seqNum, true);
                step2.OnEvent(evt, seqNum, true);

                logger.LogWarning($"#{seqNum}\t{evt.Dump()}");

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "#{Seq} Error. Message={Message}", seqNum, ex.Message);
            }
        }

        Debug.Assert(seqNum == steps);

        var portfolioManager = serviceProvider.GetRequiredService<IPortfolioManager>();
        await portfolioManager.CloseAllPositions();

        logger.LogInformation("Backtest completed.");
    }

    private static async Task LoadCandles(Guid instrumentId, ICandlesLocalStorage candleRepository, ITradingGateway tradingGateway)
    {
        if (candleRepository.GetCount(instrumentId) >= candleRepository.CandlesBufferLength)
        {
            return;
        }

        var historicCandles = await tradingGateway.GetCandles(instrumentId, candleRepository.CandlesBufferLength);
        candleRepository.Load(historicCandles);
    }
}
