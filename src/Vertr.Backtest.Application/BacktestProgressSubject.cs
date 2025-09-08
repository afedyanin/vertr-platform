using System.Reactive.Subjects;
using Vertr.Backtest.Contracts;
using Vertr.Backtest.Contracts.Interfaces;

namespace Vertr.Backtest.Application;
internal class BacktestProgressSubject : IBacktestObservable, IBacktestProgressHandler, IDisposable
{
    private readonly Subject<BacktestRun> _backtestSubject;

    public IObservable<BacktestRun> StreamBacktests() => _backtestSubject;

    public BacktestProgressSubject()
    {
        _backtestSubject = new Subject<BacktestRun>();
    }

    public Task HandleProgress(BacktestRun backtest)
    {
        _backtestSubject.OnNext(backtest);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _backtestSubject?.Dispose();
    }

}
