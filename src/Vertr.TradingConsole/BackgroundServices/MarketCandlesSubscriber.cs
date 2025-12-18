using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vertr.Common.Application;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;
using Vertr.Common.Application.Extensions;
using static StackExchange.Redis.RedisChannel;
using System.Threading.Channels;

namespace Vertr.TradingConsole.BackgroundServices;

internal sealed class MarketCandlesSubscriber : RedisServiceBase
{
    private readonly IPortfolioManager _portfolioManager;
    private readonly ICandlesLocalStorage _candleRepository;
    private readonly ITradingGateway _tradingGateway;
    private readonly IEventHandler<CandleReceivedEvent>[] _pipeline;

    private readonly Channel<Candle> _candlesChannel;
    private Task? _channelConsumerTask;
    private CancellationTokenSource? _tokenSource;

    private Instrument[] _instruments = [];
    private long _sequence;

    // TODO: Get from settings
    protected override RedisChannel RedisChannel => new RedisChannel("market.candles.*", PatternMode.Pattern);
    protected override bool IsEnabled => true;

    public MarketCandlesSubscriber(
        IServiceProvider serviceProvider,
        ILogger logger) : base(serviceProvider, logger)
    {
        _portfolioManager = serviceProvider.GetRequiredService<IPortfolioManager>();
        _candleRepository = serviceProvider.GetRequiredService<ICandlesLocalStorage>();
        _tradingGateway = serviceProvider.GetRequiredService<ITradingGateway>();
        _pipeline = ApplicationRegistrar.CreateCandleReceivedPipeline(serviceProvider);
        _sequence = 0L;
        _candlesChannel = Channel.CreateUnbounded<Candle>();
    }

    public override void HandleSubscription(RedisChannel channel, RedisValue message)
    {
        Logger.LogInformation("Received candle from cahnnel={Channel}", channel);

        var candle = Candle.FromJson(message.ToString());

        if (candle == null)
        {
            Logger.LogWarning("Cannot deserialize candle from message={Message}", message);
            return;
        }

        if (!_candlesChannel.Writer.TryWrite(candle))
        {
            Logger.LogError("Cannot write candle to channel Candle={Candle}", candle);
            return;
        }
    }

    public async Task ConsumeChannelAsync(CancellationToken cancellationToken = default)
    {
        await foreach (var candle in _candlesChannel.Reader.ReadAllAsync(cancellationToken))
        {
            try
            {
                await LoadHistoricCandlesIfReq(candle.InstrumentId);
                _candleRepository.Update(candle);

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

                Logger.LogWarning(evt.Dump());
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Candles Channel comnsumer error. Message={Message}", ex.Message);
            }
        }
    }

    protected override async ValueTask OnBeforeStart(CancellationToken cancellationToken)
    {
        _instruments = await _tradingGateway.GetAllInstruments();
        _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _channelConsumerTask = ConsumeChannelAsync(_tokenSource.Token);
    }

    private async ValueTask LoadHistoricCandlesIfReq(Guid instrumentId)
    {
        if (_candleRepository.GetCount(instrumentId) >= _candleRepository.CandlesBufferLength)
        {
            return;
        }

        var historicCandles = await _tradingGateway.GetCandles(instrumentId, _candleRepository.CandlesBufferLength);
        _candleRepository.Load(historicCandles);
    }


    public override void Dispose()
    {
        base.Dispose();
        _tokenSource?.Dispose();
        _channelConsumerTask?.Dispose();
    }
}