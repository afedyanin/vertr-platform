using Microsoft.Extensions.DependencyInjection;
using Vertr.Backtest.Contracts;
using Vertr.MarketData.Contracts;
using Vertr.Platform.Common.Channels;
using Vertr.Strategies.Contracts;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Strategies.Application.StrategiesImpl;
internal abstract class StrategyBase : IStrategy
{
    private readonly IDataProducer<TradingSignal> _tradingSignalProducer;
    private readonly ITradingSignalRepository _tradingSignalRepository;

    protected IServiceProvider ServiceProvider { get; private set; }

    public Guid Id { get; init; }

    public Guid InstrumentId { get; init; }

    public long QtyLots { get; init; }

    public Guid? BacktestId { get; private set; }

    public Guid SubAccountId { get; set; }

    protected StrategyBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        _tradingSignalProducer = ServiceProvider.GetRequiredService<IDataProducer<TradingSignal>>();
        _tradingSignalRepository = ServiceProvider.GetRequiredService<ITradingSignalRepository>();
    }

    public virtual async Task HandleMarketData(Candle candle, CancellationToken cancellationToken = default)
    {
        var tradingSignal = CreateTradingSignal(candle);

        await _tradingSignalRepository.Save(tradingSignal);
        await _tradingSignalProducer.Produce(tradingSignal, cancellationToken);
    }

    public abstract TradingSignal CreateTradingSignal(Candle candle);

    public virtual Task OnStart(
        BacktestRun? backtest = null,
        CancellationToken cancellationToken = default)
    {
        if (backtest != null)
        {
            BacktestId = backtest.Id;
            SubAccountId = backtest.SubAccountId;
        }

        return Task.CompletedTask;
    }

    public virtual Task OnStop(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public virtual void Dispose()
    {
    }
}
