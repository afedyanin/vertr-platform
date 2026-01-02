using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.Extensions;
using Vertr.Common.Contracts;
using Vertr.Common.ForecastClient;

namespace Vertr.BacktestConsole;

internal static class Program
{
    private static readonly Guid SberId = new Guid("e6123145-9665-43e0-8413-cd61b8aa9b13");

    public static async Task Main(string[] args)
    {
        var services = new ServiceCollection();

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        services
            .AddOptionsWithValidateOnStart<ThresholdSettings>()
            .Bind(configuration.GetSection(nameof(ThresholdSettings)));

        var forecastGatewayUrl = configuration.GetValue<string>("VertrForecastGateway:BaseAddress");
        Debug.Assert(!string.IsNullOrEmpty(forecastGatewayUrl));
        services.AddVertrForecastClient(forecastGatewayUrl);

        services.AddApplication();
        services.AddBacktest();

        var loggingConfig = configuration.GetSection("Logging");
        services.AddLogging(builder => builder
            .AddConfiguration(loggingConfig)
            .AddConsole());

        var serviceProvider = services.BuildServiceProvider();

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(nameof(Program));

        var historicCandlesProvider = serviceProvider.GetRequiredService<IHistoricCandlesProvider>();

        var tradingGateway = serviceProvider.GetRequiredService<ITradingGateway>();
        var instruments = await tradingGateway.GetAllInstruments();

        var predictors = configuration.GetSection("Predictors").Get<string[]>() ?? [];
        Debug.Assert(predictors.Length > 0);

        await historicCandlesProvider.Load("Data\\SBER_251101_251109.csv", SberId);

        var steps = 100;
        var candles = historicCandlesProvider.Get(SberId, skip: 100, take: steps);
        logger.LogWarning($"Init historic candles. {candles.GetCandlesRange()}");

        logger.LogInformation("Starting backtest...");

        var portfolios = await ExecuteBacktest(
            serviceProvider,
            instruments,
            predictors,
            candles,
            logger);

        logger.LogInformation("Backtest completed.");
    }

    private static async Task<IReadOnlyDictionary<string, Portfolio>> ExecuteBacktest(
        IServiceProvider serviceProvider,
        Instrument[] instruments,
        string[] predictors,
        IEnumerable<Candle> candles,
        ILogger logger)
    {
        var pipeline = serviceProvider.GetRequiredService<ICandleProcessingPipeline>();
        var portfolioRepo = serviceProvider.GetRequiredService<IPortfoliosLocalStorage>();
        var portfolioManager = serviceProvider.GetRequiredService<IPortfolioManager>();

        portfolioRepo.Init(predictors);

        pipeline.OnCandleEvent = (evt) => OnCandleEvent(evt, portfolioRepo, instruments, logger);

        await pipeline.Start();

        foreach (var candle in candles)
        {
            pipeline.Handle(candle);
        }

        await pipeline.Stop();
        await portfolioManager.CloseAllPositions();

        return portfolioRepo.GetAll();
    }

    private static ValueTask OnCandleEvent(
        CandleReceivedEvent candleReceivedEvent,
        IPortfoliosLocalStorage portfoliosLocalStorage,
        Instrument[] instruments,
        ILogger logger)
    {
        logger.LogInformation(candleReceivedEvent.Dump());

        var portfolios = portfoliosLocalStorage.GetAll();
        logger.LogInformation(DumpPortfolios(portfolios, instruments));

        return ValueTask.CompletedTask;
    }

    private static string DumpPortfolios(
        IReadOnlyDictionary<string, Portfolio> portfolios,
        Instrument[] instruments)
    {
        var sb = new StringBuilder();

        foreach (var kvp in portfolios)
        {
            sb.AppendLine(kvp.Value.Dump(kvp.Key, instruments));
        }

        return sb.ToString();
    }
}
