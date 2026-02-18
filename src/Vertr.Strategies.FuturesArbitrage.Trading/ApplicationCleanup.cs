using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;
using Vertr.Common.Application.Extensions;
using System.Collections.ObjectModel;

namespace Vertr.Strategies.FuturesArbitrage.Trading;

public class ApplicationCleanup
{
    private readonly IPortfoliosLocalStorage _portfolioRepository;
    private readonly IPortfolioManager _portfolioManager;
    private readonly ILogger<ApplicationCleanup> _logger;
    private readonly IInstrumentsLocalStorage _instrumentsRepository;

    public Action CleanupAction => OnStop;

    public ApplicationCleanup(IServiceProvider serviceProvider)
    {
        _portfolioRepository = serviceProvider.GetRequiredService<IPortfoliosLocalStorage>();
        _instrumentsRepository = serviceProvider.GetRequiredService<IInstrumentsLocalStorage>();
        _portfolioManager = serviceProvider.GetRequiredService<IPortfolioManager>();

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        _logger = loggerFactory.CreateLogger<ApplicationCleanup>();
    }

    private void OnStop()
    {
        _logger.LogInformation("ApplicationStopped. Performing cleanup...");

        _logger.LogInformation("01 Closing all positions");
        //_portfolioManager.CloseAllPositions().GetAwaiter().GetResult();

        _logger.LogInformation("02 Processing orders...");
        //Task.Delay(5000, CancellationToken.None).GetAwaiter().GetResult();

        _logger.LogInformation("03 Dumping portfolios...");
        var instruments = _instrumentsRepository.GetAll();
        var portfolios = _portfolioRepository.GetAll();
        var dump = DumpPortfolios(portfolios, instruments);
        _logger.LogWarning(dump);

        _logger.LogInformation("04 Cleanup completed.");
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
