using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vertr.Adapters.Tinvest;
using Vertr.Domain;
using Vertr.Domain.Ports;

namespace Vertr.Application.Candles;
internal class UpdateLastCandlesHandler : IRequestHandler<UpdateLastCandlesRequest>
{
    private readonly ITinvestCandlesRepository _repository;
    private readonly ITinvestGateway _gateway;
    private readonly TinvestSettings _tinvestSettings;

    private readonly ILogger<UpdateLastCandlesHandler> _logger;

    public UpdateLastCandlesHandler(
        ITinvestCandlesRepository repository,
        ITinvestGateway gateway,
        IOptions<TinvestSettings> settings,
        ILogger<UpdateLastCandlesHandler> logger)
    {
        _repository = repository;
        _gateway = gateway;
        _logger = logger;
        _tinvestSettings = settings.Value;
    }

    public async Task Handle(UpdateLastCandlesRequest request, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();

        foreach (var symbol in request.Symbols)
        {
            tasks.Add(LoadSymbol(symbol.Trim(), request.Interval, cancellationToken));
        }

        await Task.WhenAll(tasks);

        _logger.LogDebug($"Loading T-Invest candles for {request.Symbols.Count()} symbols completed.");
    }

    internal async Task LoadSymbol(string symbol, CandleInterval interval, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrEmpty(symbol))
        {
            throw new ArgumentNullException(nameof(symbol));
        }

        try
        {
            var symbolId = _tinvestSettings.GetSymbolId(symbol) ??
                throw new ArgumentException($"Cannot find instrumentId for {symbol}");

            (DateTime from, DateTime to) = await GetLoadingInterval(symbol, interval);

            _logger.LogDebug($"Loading candles {symbol} {interval} from={from:O} to={to:O}");
            var candles = await _gateway.GetCandles(symbolId, interval, from, to);

            if (candles == null || !candles.Any())
            {
                _logger.LogDebug($"No new candles found for {symbol} {interval}");
                return;
            }

            var count = await _repository.Insert(symbol, interval, candles);
            _logger.LogDebug($"{count} candles inserted/updated for {symbol} {interval}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "T-Invest candles loading error.");
        }
    }

    internal async Task<(DateTime from, DateTime to)> GetLoadingInterval(string symbol, CandleInterval interval)
    {
        var dateTo = DateTime.UtcNow;
        var dateFrom = dateTo.AddDays(-7);

        // adjust dateFrom from last known candle
        var dbCandles = await _repository.GetLast(symbol, interval, count: 1, completedOnly: true);
        var lastCandleTime = dbCandles.Select(c => c.TimeUtc).FirstOrDefault();
        _logger.LogDebug($"Last known candle time={lastCandleTime:O}. {symbol} {interval}");
        dateFrom = lastCandleTime > dateFrom ? lastCandleTime : dateFrom;

        return (dateFrom, dateTo);
    }
}
