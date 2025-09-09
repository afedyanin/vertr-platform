using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Vertr.Backtest.Contracts;
using Vertr.Backtest.Contracts.Interfaces;
using Vertr.Platform.Common.Extensions;

namespace Vertr.Backtest.WebApi.Hubs;

public class BacktestProgressHub : Hub
{
    private readonly IBacktestObservable _backtestObservable;

    public BacktestProgressHub(
        IBacktestObservable backtestObservable)
    {
        _backtestObservable = backtestObservable;
    }

    public ChannelReader<BacktestRun> StreamBacktestsProgress()
    {
        return _backtestObservable.StreamBacktests().AsChannelReader(10);
    }
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }
}
