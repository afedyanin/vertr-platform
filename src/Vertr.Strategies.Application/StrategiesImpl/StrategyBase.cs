using Microsoft.Extensions.DependencyInjection;
using Vertr.Backtest.Contracts;
using Vertr.MarketData.Contracts;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.Platform.Common.Mediator;
using Vertr.Strategies.Contracts;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Strategies.Application.StrategiesImpl;
internal abstract class StrategyBase : IStrategy
{
    private readonly ITradingSignalRepository _tradingSignalRepository;
    private readonly IMediator _mediator;

    private Candle? _lastProcessedCandle;

    protected IServiceProvider ServiceProvider { get; private set; }

    public Guid Id { get; init; }

    public Guid InstrumentId { get; init; }

    public long QtyLots { get; init; }

    public Guid? BacktestId { get; private set; }

    public Guid PortfolioId { get; set; }

    protected StrategyBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        _tradingSignalRepository = ServiceProvider.GetRequiredService<ITradingSignalRepository>();
        _mediator = ServiceProvider.GetRequiredService<IMediator>();
    }

    public virtual async Task HandleMarketData(Candle candle, CancellationToken cancellationToken = default)
    {
        _lastProcessedCandle = candle;

        var tradingSignal = CreateTradingSignal(candle);

        if (!tradingSignal.BacktestId.HasValue)
        {
            _ = await _tradingSignalRepository.Save(tradingSignal);
        }

        var command = new TradingSignalCommand
        {
            RequestId = tradingSignal.Id,
            InstrumentId = tradingSignal.InstrumentId,
            QtyLots = tradingSignal.QtyLots,
            PortfolioId = tradingSignal.PortfolioId,
            BacktestId = tradingSignal.BacktestId,
            CreatedAt = tradingSignal.CreatedAt,
            Price = tradingSignal.Price,
        };

        _ = await _mediator.Send(command, cancellationToken);
    }

    public abstract TradingSignal CreateTradingSignal(Candle candle);

    public virtual Task OnStart(
        BacktestRun? backtest = null,
        CancellationToken cancellationToken = default)
    {
        if (backtest != null)
        {
            BacktestId = backtest.Id;
            PortfolioId = backtest.PortfolioId;
        }

        return Task.CompletedTask;
    }

    public virtual async Task OnStop(CancellationToken cancellationToken = default)
    {
        var command = new ClosePositionCommand
        {
            RequestId = Guid.NewGuid(),
            InstrumentId = InstrumentId,
            PortfolioId = PortfolioId,
            BacktestId = BacktestId,
            CreatedAt = _lastProcessedCandle?.TimeUtc ?? DateTime.UtcNow,
            Price = _lastProcessedCandle?.Close ?? decimal.Zero,
        };

        await _mediator.Send(command);
    }

    public virtual void Dispose()
    {
    }
}
