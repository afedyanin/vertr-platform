using Vertr.Backtest.Contracts;
using Vertr.MarketData.Contracts;

namespace Vertr.Strategies.Contracts.Interfaces;

public interface IStrategy : IDisposable
{
    public Guid Id { get; }

    public Guid InstrumentId { get; }

    public Guid? BacktestId { get; }

    public Guid SubAccountId { get; }

    public long QtyLots { get; }

    public Task HandleMarketData(Candle candle, CancellationToken cancellationToken = default);

    public Task OnStart(BacktestRun? backtest = null, CancellationToken cancellationToken = default);

    public Task OnStop(CancellationToken cancellationToken = default);
}
