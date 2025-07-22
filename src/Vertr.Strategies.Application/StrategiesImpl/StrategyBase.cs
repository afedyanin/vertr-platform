using Microsoft.Extensions.DependencyInjection;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts;
using Vertr.Platform.Common.Channels;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.Strategies.Application.StrategiesImpl;
internal abstract class StrategyBase
{
    private readonly IDataProducer<TradingSignal> _tradingSignalProducer;

    protected IServiceProvider ServiceProvider { get; private set; }

    protected IMarketDataRepository MarketDataRepository { get; private set; }

    public Guid Id { get; init; }

    public Guid InstrumentId { get; init; }

    public PortfolioIdentity PortfolioIdentity { get; init; }

    public long QtyLots { get; init; }

    protected StrategyBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        MarketDataRepository = ServiceProvider.GetRequiredService<IMarketDataRepository>();
        _tradingSignalProducer = ServiceProvider.GetRequiredService<IDataProducer<TradingSignal>>();
    }

    public virtual async Task HandleMarketData(Candle candle, CancellationToken cancellationToken = default)
    {
        var tradingSignal = CreateTradingSignal(candle);
        await _tradingSignalProducer.Produce(tradingSignal, cancellationToken);
    }

    public abstract TradingSignal CreateTradingSignal(Candle candle);
}
