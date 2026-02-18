using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vertr.Clients.MoexApiClient;
using Vertr.Common.Application.Abstractions;

namespace Vertr.Strategies.FuturesArbitrage.Trading.BackgroundServices;

internal sealed class StaticDataLoaderService : BackgroundService
{
    private readonly IMoexApiClient _moexApiClient;
    private readonly ITradingGateway _tinvestGateway;
    private readonly IInstrumentsLocalStorage _instrumentsLocalStorage;
    private readonly IFutureInfoRepository _futureInfoRepository;
    private readonly IIndexRatesRepository _indexRatesRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;

    public StaticDataLoaderService(
        IConfiguration configuration,
        ITradingGateway tinvestGateway,
        IMoexApiClient moexApiClient,
        IInstrumentsLocalStorage instrumentsLocalStorage,
        IFutureInfoRepository futureInfoRepository,
        IIndexRatesRepository indexRatesRepository,
        ILoggerFactory loggerFactory)
    {
        _tinvestGateway = tinvestGateway;
        _moexApiClient = moexApiClient;
        _instrumentsLocalStorage = instrumentsLocalStorage;
        _futureInfoRepository = futureInfoRepository;
        _indexRatesRepository = indexRatesRepository;
        _configuration = configuration;
        _logger = loggerFactory.CreateLogger(nameof(StaticDataLoaderService));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Load instruments
        var instruments = await _tinvestGateway.GetAllInstruments();
        _instrumentsLocalStorage.Load(instruments);
        _logger.LogInformation("{InstrumentCount} instruments loaded.", instruments.Length);

        var futureTickers = instruments
            .Where(i => i.InstrumentType == "futures")
            .Select(i => i.Ticker)
            .ToArray();

        // Load FutureInfos
        var allFutures = await _moexApiClient.GetFutureInfo(futureTickers);
        var futures = allFutures.ToArray() ?? [];
        _futureInfoRepository.Load(futures);
        _logger.LogInformation("{FuturesCount} futures loaded.", futures.Length);

        // Load Index Rates
        var indexRates = _configuration.GetSection("IndexRates").Get<string[]>() ?? [];

        foreach (var indexRate in indexRates)
        {
            var allRates = await _moexApiClient.GetIndexRates(indexRate);
            var rates = allRates.ToArray() ?? [];
            _indexRatesRepository.Load(rates);
        }

        _logger.LogInformation("{RatesCount} index rates loaded.", indexRates.Length);

        // TODO: Periodically update Index Rates
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
