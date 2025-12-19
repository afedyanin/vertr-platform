using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.EventHandlers;
using Vertr.Common.Application.Extensions;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Services;

internal class CandleProcessingPipeline : ICandleProcessingPipeline, IDisposable
{
    private readonly ITradingGateway _tradingGateway;
    private readonly ICandlesLocalStorage _candlesLocalStorage;
    private readonly IPortfoliosLocalStorage _portfoliosLocalStorage;
    private readonly IPortfolioManager _portfolioManager;

    private readonly IEventHandler<CandleReceivedEvent>[] _pipeline = [];
    private readonly ILogger<CandleProcessingPipeline> _logger;

    private readonly Channel<Candle> _candlesChannel;

    private Task? _channelConsumerTask;
    private CancellationTokenSource? _tokenSource;

    private long _sequence;
    private Instrument[] _instruments = [];

    public CandleProcessingPipeline(
        IServiceProvider serviceProvider,
        ILogger<CandleProcessingPipeline> logger)
    {
        _tradingGateway = serviceProvider.GetRequiredService<ITradingGateway>();
        _candlesLocalStorage = serviceProvider.GetRequiredService<ICandlesLocalStorage>();
        _portfoliosLocalStorage = serviceProvider.GetRequiredService<IPortfoliosLocalStorage>();
        _portfolioManager = serviceProvider.GetRequiredService<IPortfolioManager>();
        _logger = logger;

        _pipeline = CreatePipeline(serviceProvider);
        _candlesChannel = Channel.CreateUnbounded<Candle>();
        _sequence = 0L;
    }

    public virtual void Handle(Candle candle)
    {
        if (!_candlesChannel.Writer.TryWrite(candle))
        {
            _logger.LogError("Cannot write candle to channel Candle={Candle}", candle);
            return;
        }
    }

    public virtual async ValueTask OnBeforeStart(CancellationToken cancellationToken = default)
    {
        _instruments = await _tradingGateway.GetAllInstruments();
        _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _channelConsumerTask = ConsumeChannelAsync(_tokenSource.Token);
    }

    public virtual async ValueTask OnBeforeStop()
    {
        _logger.LogWarning("Closing portfolios...");
        await _portfolioManager.CloseAllPositions();

        var dump = await DumpPortfolios();
        _logger.LogWarning(dump);
    }

    protected virtual async Task ConsumeChannelAsync(CancellationToken cancellationToken = default)
    {
        await foreach (var candle in _candlesChannel.Reader.ReadAllAsync(cancellationToken))
        {
            try
            {
                await LoadHistoricCandlesIfReq(candle.InstrumentId);
                _candlesLocalStorage.Update(candle);

                var evt = new CandleReceivedEvent
                {
                    Sequence = _sequence++,
                    Candle = candle,
                    Instrument = _instruments.GetById(candle.InstrumentId)!,
                };

                foreach (var handler in _pipeline)
                {
                    await handler.OnEvent(evt);
                }

                _logger.LogWarning(evt.Dump());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Candles Channel comnsumer error. Message={Message}", ex.Message);
            }
        }
    }

    private async ValueTask LoadHistoricCandlesIfReq(Guid instrumentId)
    {
        if (_candlesLocalStorage.GetCount(instrumentId) >= _candlesLocalStorage.CandlesBufferLength)
        {
            return;
        }

        var historicCandles = await _tradingGateway.GetCandles(instrumentId, _candlesLocalStorage.CandlesBufferLength);
        _candlesLocalStorage.Load(historicCandles);
    }

    private IEventHandler<CandleReceivedEvent>[] CreatePipeline(IServiceProvider serviceProvider)
    {
        var pipeline = new IEventHandler<CandleReceivedEvent>[4];
        pipeline[0] = serviceProvider.GetRequiredService<MarketDataPredictor>();
        pipeline[1] = serviceProvider.GetRequiredService<TradingSignalsGenerator>();
        pipeline[2] = serviceProvider.GetRequiredService<PortfolioPositionHandler>();
        pipeline[3] = serviceProvider.GetRequiredService<OrderExecutionHandler>();

        return pipeline;
    }

    private async Task<string> DumpPortfolios()
    {
        var portfolios = _portfoliosLocalStorage.GetAll();

        var sb = new StringBuilder();

        foreach (var kvp in portfolios)
        {
            sb.AppendLine(kvp.Value.Dump(kvp.Key, _instruments, verbose: false));
        }

        return sb.ToString();
    }


    public void Dispose()
    {
        _tokenSource?.Dispose();
        _channelConsumerTask?.Dispose();
    }
}
