using System.Data;
using System.Diagnostics;
using System.Text;
using Microsoft.Data.Analysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.Extensions;
using Vertr.Common.Application.LocalStorage;
using Vertr.Common.Contracts;
using Vertr.Common.ForecastClient;

namespace Vertr.BacktestConsole;

internal static class Program
{
    private static readonly Guid SberId = new Guid("e6123145-9665-43e0-8413-cd61b8aa9b13");
    private static readonly Guid RubId = new Guid("a92e2e25-a698-45cc-a781-167cf465257c");

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

        var steps = 60 * 8;
        var testCounts = new int[]
        {
            100,
            200,
            300
        };

        await historicCandlesProvider.Load("Data\\SBER_251112_251226.csv", SberId);
        var totalRange = historicCandlesProvider.GetRange(SberId);

        if (totalRange == null ||
            totalRange.Value.Count < (CandlesLocalStorage.CandlesBufferLength + steps * 2))
        {
            logger.LogError("Insufficient historic candles for backtest.");
            return;
        }

        var btStatsDict = new Dictionary<int, Dictionary<string, PortfolioInstrumentStats>>();

        foreach (var testSize in testCounts)
        {
            logger.LogInformation("Executing test size={TestSize}.", testSize);

            var portfoliosByPredictor = new Dictionary<string, List<Portfolio>>();

            for (var i = 0; i < testSize; i++)
            {
                var maxSeed = CandlesLocalStorage.CandlesBufferLength + steps;
                var randomSeed = Random.Shared.Next(0, maxSeed);

                var portfolios = await ExecuteBacktest(
                    serviceProvider,
                    steps,
                    instruments,
                    predictors,
                    logger,
                    randomSeed);

                foreach (var item in portfolios)
                {
                    portfoliosByPredictor.TryGetValue(item.Key, out var portfolioList);

                    if (portfolioList == null)
                    {
                        portfolioList = [];
                        portfoliosByPredictor[item.Key] = portfolioList;
                    }

                    portfolioList.Add(item.Value);
                }
            }

            var statsDict = new Dictionary<string, PortfolioInstrumentStats>();

            foreach (var kvp in portfoliosByPredictor)
            {
                statsDict[kvp.Key] = kvp.Value.StatsByInstrument(RubId);
            }

            btStatsDict[testSize] = statsDict;

            logger.LogInformation("Test result for test size={TestSize}", testSize);

            foreach (var kvp in statsDict)
            {
                logger.LogInformation($"{kvp.Key}: {kvp.Value}");
            }
        }

        var df = CreateDataFrame(btStatsDict);
        DataFrame.SaveCsv(df, "Data\\bt_ml_251112_251226.csv");

        logger.LogInformation("Backtest completed.");
    }

    private static async Task<IReadOnlyDictionary<string, Portfolio>> ExecuteBacktest(
        IServiceProvider serviceProvider,
        int steps,
        Instrument[] instruments,
        string[] predictors,
        ILogger logger,
        int? seed = null)
    {
        var candlesLocalStorage = serviceProvider.GetRequiredService<ICandlesLocalStorage>();
        var historicCandlesProvider = serviceProvider.GetRequiredService<IHistoricCandlesProvider>();

        seed ??= 0;

        var bufferCandles = historicCandlesProvider.Get(
            SberId, skip: seed.Value, take: CandlesLocalStorage.CandlesBufferLength);

        candlesLocalStorage.Clear();
        candlesLocalStorage.Fill(bufferCandles);

        var candles = historicCandlesProvider.Get(
            SberId, skip: seed.Value + CandlesLocalStorage.CandlesBufferLength, take: steps);

        logger.LogDebug($"Init historic candles. {candles.GetCandlesRange()}");

        var pipeline = serviceProvider.GetRequiredService<ICandleProcessingPipeline>();
        var portfolioRepo = serviceProvider.GetRequiredService<IPortfoliosLocalStorage>();
        var portfolioManager = serviceProvider.GetRequiredService<IPortfolioManager>();

        portfolioRepo.Init(predictors);

        //pipeline.OnCandleEvent = (evt) => OnCandleEvent(evt, portfolioRepo, instruments, logger);

        await pipeline.Start();

        foreach (var candle in candles)
        {
            pipeline.Handle(candle);
        }

        await pipeline.Stop();
        await portfolioManager.CloseAllPositions();

        return portfolioRepo.GetAll();
    }

    // https://swharden.com/blog/2022-05-01-dotnet-dataframe/
    private static DataFrame CreateDataFrame(Dictionary<int, Dictionary<string, PortfolioInstrumentStats>> btStatsDict)
    {
        var columns = new Dictionary<string, List<double>>()
        {
            { "batch_size", new List<double>() }
        };

        foreach (var kvBatch in btStatsDict)
        {
            columns["batch_size"].Add(kvBatch.Key);

            foreach ((var colName, var colValue) in ToColumns(kvBatch.Value))
            {
                columns.TryGetValue(colName, out var colValues);

                if (colValues == null)
                {
                    colValues = [];
                    columns[colName] = colValues;
                }

                colValues.Add(colValue);
            }
        }

        var dfCols = columns.Select(kvp => new DoubleDataFrameColumn(kvp.Key, kvp.Value));
        var df = new DataFrame(dfCols);

        return df;
    }

    private static IEnumerable<(string, double)> ToColumns(Dictionary<string, PortfolioInstrumentStats> statsDict)
    {
        var cols = new List<(string, double)>();

        foreach (var kvp in statsDict)
        {
            cols.AddRange(kvp.Value.ToColumns(kvp.Key));
        }

        return cols;
    }

    private static ValueTask OnCandleEvent(
        CandleReceivedEvent candleReceivedEvent,
        IPortfoliosLocalStorage portfoliosLocalStorage,
        Instrument[] instruments,
        ILogger logger)
    {

        if (candleReceivedEvent.OrderRequests.Any())
        {
            logger.LogInformation(candleReceivedEvent.Dump());
        }

        //var portfolios = portfoliosLocalStorage.GetAll();
        //logger.LogInformation(DumpPortfolios(portfolios, instruments));

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
