using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vertr.Common.Application;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;
using Vertr.Common.Application.Extensions;
using static StackExchange.Redis.RedisChannel;

namespace Vertr.TradingConsole.BackgroundServices;

internal sealed class MarketCandlesSubscriber : RedisServiceBase
{
    private readonly IPortfolioManager _portfolioManager;
    private readonly ICandlesLocalStorage _candleRepository;
    private readonly ITradingGateway _tradingGateway;
    private readonly IEventHandler<CandleReceivedEvent>[] _pipeline;

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
    }

    protected override async ValueTask OnBeforeStart()
    {
        _instruments = await _tradingGateway.GetAllInstruments();
    }

    protected override async ValueTask OnBeforeStop()
    {
        // TODO: Use settings
        await _portfolioManager.CloseAllPositions();
        await base.OnBeforeStop();
    }
}