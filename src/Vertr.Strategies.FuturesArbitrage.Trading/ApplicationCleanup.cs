using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;
using Vertr.Common.Application.Extensions;
using System.Collections.ObjectModel;
using StackExchange.Redis;

namespace Vertr.Strategies.FuturesArbitrage.Trading;

public class ApplicationCleanup
{
    private const string PortfoliosKey = "portfolios";

    private readonly IPortfoliosLocalStorage _portfolioRepository;
    private readonly IPortfolioManager _portfolioManager;
    private readonly ILogger<ApplicationCleanup> _logger;
    private readonly IInstrumentsLocalStorage _instrumentsRepository;
    private readonly IConnectionMultiplexer _redis;

    private IDatabase GetDatabase() => _redis.GetDatabase();

    public Action CleanupAction => OnStop;

    public ApplicationCleanup(IServiceProvider serviceProvider)
    {
        _portfolioRepository = serviceProvider.GetRequiredService<IPortfoliosLocalStorage>();
        _instrumentsRepository = serviceProvider.GetRequiredService<IInstrumentsLocalStorage>();
        _portfolioManager = serviceProvider.GetRequiredService<IPortfolioManager>();
        _redis = serviceProvider.GetRequiredService<IConnectionMultiplexer>();

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        _logger = loggerFactory.CreateLogger<ApplicationCleanup>();
    }

    private void OnStop()
    {
        _logger.LogInformation("ApplicationStopped. Performing cleanup...");

        _logger.LogInformation("01 Closing all positions");

        // TODO Use this requests to collect trading stats info
        //var requests = _portfolioManager.CloseAllPositions().GetAwaiter().GetResult();
        _ = _portfolioManager.CloseAllPositions().GetAwaiter().GetResult();

        _logger.LogInformation("02 Processing orders...");
        Task.Delay(5000, CancellationToken.None).GetAwaiter().GetResult();

        _logger.LogInformation("03 Dumping portfolios...");
        var instruments = _instrumentsRepository.GetAll();

        var portfolios = UpdatePortfoliosFromRedis(_portfolioRepository.GetAll());
        var dump = DumpPortfolios(portfolios, instruments);
        _logger.LogWarning(dump);

        _logger.LogInformation("04 Cleanup completed.");
    }

    private ReadOnlyDictionary<string, Portfolio> UpdatePortfoliosFromRedis(ReadOnlyDictionary<string, Portfolio> oldPortfolios)
    {
        var updated = GetPortfoliosFromRedis();
        var res = new Dictionary<string, Portfolio>();

        foreach (var kvp in oldPortfolios)
        {
            var found = updated.FirstOrDefault(p => p.Id == kvp.Value.Id);

            if (found != null)
            {
                res[kvp.Key] = found;
            }
        }

        return res.AsReadOnly();
    }

    private Portfolio[] GetPortfoliosFromRedis()
    {
        var entries = GetDatabase().HashGetAll(PortfoliosKey);
        var res = new List<Portfolio>();

        foreach (var entry in entries)
        {
            if (string.IsNullOrEmpty(entry.Value))
            {
                continue;
            }

            var portfollio = Portfolio.FromJson(entry.Value!);
            if (portfollio == null)
            {
                continue;
            }

            res.Add(portfollio);
        }

        return [.. res];
    }

    private static string DumpPortfolios(
        ReadOnlyDictionary<string, Portfolio> portfolios,
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
