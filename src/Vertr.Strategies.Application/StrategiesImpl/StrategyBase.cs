using Microsoft.Extensions.DependencyInjection;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces.old;
using Vertr.Strategies.Contracts;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Strategies.Application.StrategiesImpl;
internal abstract class StrategyBase : IStrategy
{
    //private readonly IDataProducer<TradingSignal> _tradingSignalProducer;
    private readonly ITradingSignalRepository _tradingSignalRepository;

    protected IServiceProvider ServiceProvider { get; private set; }

    protected IMarketDataRepository MarketDataRepository { get; private set; }


    public Guid Id { get; init; }

    public Guid InstrumentId { get; init; }

    public long QtyLots { get; init; }

    public Guid? BacktestId { get; init; }

    public Guid SubAccountId { get; init; }

    protected StrategyBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        MarketDataRepository = ServiceProvider.GetRequiredService<IMarketDataRepository>();
        //_tradingSignalProducer = ServiceProvider.GetRequiredService<IDataProducer<TradingSignal>>();
        _tradingSignalRepository = ServiceProvider.GetRequiredService<ITradingSignalRepository>();
    }

    public virtual async Task HandleMarketData(Candle candle, CancellationToken cancellationToken = default)
    {
        var tradingSignal = CreateTradingSignal(candle);

        // TODO: Replace with WhenAll?
        await _tradingSignalRepository.Save(tradingSignal);
        //await _tradingSignalProducer.Produce(tradingSignal, cancellationToken);
    }

    public abstract TradingSignal CreateTradingSignal(Candle candle);
}
