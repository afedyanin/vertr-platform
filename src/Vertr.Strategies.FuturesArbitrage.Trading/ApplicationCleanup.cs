using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.Extensions;
using Vertr.Common.Contracts;
using Vertr.Strategies.FuturesArbitrage.Abstractions;
using Vertr.Strategies.FuturesArbitrage.Models;

namespace Vertr.Strategies.FuturesArbitrage.Trading;

public class ApplicationCleanup
{
    private readonly IPortfoliosLocalStorage _portfolioRepository;
    private readonly IPortfolioManager _portfolioManager;
    private readonly ILogger<ApplicationCleanup> _logger;
    private readonly IInstrumentsLocalStorage _instrumentsRepository;
    private readonly ITradingGateway _tradingGateway;
    private readonly ITradingStatsLocalStorage _tradingStatsLocalStorage;

    public Action CleanupAction => OnStop;

    public ApplicationCleanup(IServiceProvider serviceProvider)
    {
        _portfolioRepository = serviceProvider.GetRequiredService<IPortfoliosLocalStorage>();
        _instrumentsRepository = serviceProvider.GetRequiredService<IInstrumentsLocalStorage>();
        _portfolioManager = serviceProvider.GetRequiredService<IPortfolioManager>();
        _tradingGateway = serviceProvider.GetRequiredService<ITradingGateway>();
        _tradingStatsLocalStorage = serviceProvider.GetRequiredService<ITradingStatsLocalStorage>();

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

        _logger.LogInformation("04 Dumping trading stats...");

        var tradingStats = _tradingStatsLocalStorage.GetAll();
        foreach (var item in tradingStats)
        {
            if (!string.IsNullOrEmpty(item.OrderExecutionInfo.OrderId))
            {
                var orderTrades = _tradingGateway.FindOrderTrades(item.OrderExecutionInfo.OrderId).GetAwaiter().GetResult();
                if (orderTrades != null)
                {
                    item.OrderTrades = orderTrades;
                }
            }
        }

        var tradingDump = DumpTradingStats(tradingStats);
        _logger.LogWarning(tradingDump);

        _logger.LogInformation("05 Cleanup completed.");
    }

    private ReadOnlyDictionary<string, Portfolio> UpdatePortfoliosFromRedis(ReadOnlyDictionary<string, Portfolio> oldPortfolios)
    {
        var res = new Dictionary<string, Portfolio>();

        foreach (var kvp in oldPortfolios)
        {
            var found = _tradingGateway.GetPortfolio(kvp.Value.Id).GetAwaiter().GetResult();

            if (found != null)
            {
                res[kvp.Key] = found;
            }
        }

        return res.AsReadOnly();
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

    // TODO: Dump to csv file
    private static string DumpTradingStats(IEnumerable<TradingStatsInfo> tradingStats)
    {
        var sb = new StringBuilder();

        foreach (var item in tradingStats)
        {
            sb.AppendLine(JsonSerializer.Serialize(item));
        }

        return sb.ToString();
    }
}
