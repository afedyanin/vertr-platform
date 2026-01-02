using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.EventHandlers;
using Vertr.Common.Application.Extensions;
using Vertr.Common.Application.LocalStorage;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Services;

internal class CandleProcessingPipeline : ICandleProcessingPipeline
{
    private readonly ITradingGateway _tradingGateway;
    private readonly ICandlesLocalStorage _candlesLocalStorage;
    private readonly ILogger<CandleProcessingPipeline> _logger;

    private readonly IEventHandler<CandleReceivedEvent>[] _pipeline = [];
    private readonly Channel<Candle> _candlesChannel;

    private Task? _channelConsumerTask;
    private Instrument[] _instruments = [];
    private long _sequence;

    public Func<CandleReceivedEvent, ValueTask>? OnCandleEvent { get; set; }

    public CandleProcessingPipeline(IServiceProvider serviceProvider)
    {
        _tradingGateway = serviceProvider.GetRequiredService<ITradingGateway>();
        _candlesLocalStorage = serviceProvider.GetRequiredService<ICandlesLocalStorage>();

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        _logger = loggerFactory.CreateLogger<CandleProcessingPipeline>();

        _pipeline = CreatePipeline(serviceProvider);
        _candlesChannel = Channel.CreateUnbounded<Candle>();
        _sequence = 0L;
    }

    public void Handle(Candle candle)
    {
        if (!_candlesChannel.Writer.TryWrite(candle))
        {
            _logger.LogError("Cannot write candle to channel Candle={Candle}", candle);
            return;
        }
    }

    public async Task Start(CancellationToken cancellationToken = default)
    {
        _instruments = await _tradingGateway.GetAllInstruments();
        _channelConsumerTask = ConsumeChannelAsync(cancellationToken);
    }

    public Task Stop()
    {
        _candlesChannel.Writer.Complete();
        return _channelConsumerTask ?? Task.CompletedTask;
    }

    private async Task ConsumeChannelAsync(CancellationToken cancellationToken = default)
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

                if (OnCandleEvent != null)
                {
                    await OnCandleEvent(evt);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Candles Channel comnsumer error. Message={Message}", ex.Message);
            }
        }
    }

    private async ValueTask LoadHistoricCandlesIfReq(Guid instrumentId)
    {
        if (_candlesLocalStorage.Any(instrumentId))
        {
            return;
        }

        var historicCandles = await _tradingGateway.GetCandles(instrumentId, CandlesLocalStorage.CandlesBufferLength);
        _candlesLocalStorage.Fill(historicCandles);
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
}
