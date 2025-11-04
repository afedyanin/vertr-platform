using Vertr.Backtest.Contracts;
using Vertr.MarketData.Contracts;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.CliTools.Tests.Model;

internal class SimpleStrategy : IStrategy
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public Guid InstrumentId => Guid.Empty;

    public Guid? BacktestId { get; private set; }

    public Guid PortfolioId => Guid.Empty;

    public long QtyLots => 1;

    public Task HandleMarketData(Candle candle, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"SimpleStrategy processing candle={candle}");
        return Task.CompletedTask;
    }

    public Task OnStart(BacktestRun? backtest = null, CancellationToken cancellationToken = default)
    {
        BacktestId = backtest?.Id;
        Console.WriteLine($"Starting SimpleStrategy Id={Id}");
        return Task.CompletedTask;
    }

    public Task OnStop(CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Stopping SimpleStrategy Id={Id}");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
    }
}
