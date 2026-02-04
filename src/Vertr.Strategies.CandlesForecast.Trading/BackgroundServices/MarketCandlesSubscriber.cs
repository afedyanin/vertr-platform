using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.Configuration;
using Vertr.Common.Contracts;
using Vertr.Strategies.CandlesForecast.Abstractions;
using Vertr.Strategies.CandlesForecast.Extensions;
using Vertr.Strategies.Hosting.BackgroundServices;
using static StackExchange.Redis.RedisChannel;

namespace Vertr.Strategies.CandlesForecast.Trading.BackgroundServices;

internal sealed class MarketCandlesSubscriber : RedisServiceBase
{
    private readonly ICandleProcessingPipeline _candleProcessingPipeline;
    private readonly IPortfolioManager _portfolioManager;
    private readonly ILogger<RedisServiceBase> _logger;
    private readonly Channel<CandleReceivedEvent> _eventChannel = Channel.CreateUnbounded<CandleReceivedEvent>();
    private readonly InstrumentSettings _instrumentSettings;

    private Task? _channelConsumerTask;
    private int _eventSequence;

    protected override RedisChannel RedisChannel => new RedisChannel(Subscriptions.Candles.Channel, PatternMode.Pattern);
    protected override bool IsEnabled => Subscriptions.Candles.IsEnabled;

    public MarketCandlesSubscriber(IServiceProvider serviceProvider, IConfiguration configuration) : base(serviceProvider, configuration)
    {
        _candleProcessingPipeline = serviceProvider.GetRequiredService<ICandleProcessingPipeline>();
        _portfolioManager = serviceProvider.GetRequiredService<IPortfolioManager>();
        _logger = LoggerFactory.CreateLogger<MarketCandlesSubscriber>();
        _instrumentSettings = serviceProvider.GetRequiredService<IOptions<InstrumentSettings>>().Value;
    }

    public override void HandleSubscription(RedisChannel channel, RedisValue message)
    {
        _logger.LogInformation("Received candle from cahnnel={Channel}", channel);

        var candle = Candle.FromJson(message.ToString());

        if (candle == null)
        {
            _logger.LogWarning("Cannot deserialize candle from message={Message}", message);
            return;
        }

        if (_instrumentSettings.BasicAsset != candle.InstrumentId)
        {
            _logger.LogDebug("Skipping InstrumentId={InstrumentId}", candle.InstrumentId);
            return;
        }

        var evt = new CandleReceivedEvent
        {
            Sequence = _eventSequence++,
            Candle = candle,
        };

        _logger.LogInformation("#{Sequence} Candle={Candle}", evt.Sequence, evt.Candle.Dump());
        _eventChannel.Writer.TryWrite(evt);
    }

    private async Task ConsumeChannelAsync(CancellationToken cancellationToken = default)
    {
        await foreach (var tEvent in _eventChannel.Reader.ReadAllAsync(cancellationToken))
        {
            try
            {
                await _candleProcessingPipeline.Handle(tEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing event={Event} Message={Message}", tEvent, ex.Message);
            }
        }
    }

    protected override async ValueTask OnBeforeStart(CancellationToken cancellationToken)
    {
        await base.OnBeforeStart(cancellationToken);
        _eventSequence = 0;
        _channelConsumerTask = ConsumeChannelAsync(cancellationToken);
        await _candleProcessingPipeline.Start(cancellationToken);
    }

    protected override async ValueTask OnBeforeStop()
    {
        await _portfolioManager.CloseAllPositions();
        await base.OnBeforeStop();
    }
}