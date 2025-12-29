using System.Diagnostics;
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

        var portfolioRepo = serviceProvider.GetRequiredService<IPortfoliosLocalStorage>();
        var historicCandlesProvider = serviceProvider.GetRequiredService<IHistoricCandlesProvider>();
        var pipeline = serviceProvider.GetRequiredService<ICandleProcessingPipeline>();

        var predictors = configuration.GetSection("Predictors").Get<string[]>() ?? [];
        Debug.Assert(predictors.Length > 0);
        portfolioRepo.Init(predictors);

        var steps = 100;
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
    }
}
