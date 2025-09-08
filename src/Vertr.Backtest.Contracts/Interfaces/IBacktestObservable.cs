namespace Vertr.Backtest.Contracts.Interfaces;
public interface IBacktestObservable
{
    public IObservable<BacktestRun> StreamBacktests();
}
