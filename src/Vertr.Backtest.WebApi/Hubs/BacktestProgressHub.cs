using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Vertr.Backtest.Contracts;
using Vertr.Backtest.Contracts.Interfaces;
using Vertr.Platform.Common.Extensions;

namespace Vertr.Backtest.WebApi.Hubs;

public class BacktestProgressHub : Hub
{
    private readonly IBacktestObservable _stockTickerObservable;

    public BacktestProgressHub(
        IBacktestObservable stockTickerObservable)
    {
        _stockTickerObservable = stockTickerObservable;
    }

    public ChannelReader<BacktestRun> StreamBacktestsProgress()
    {
        return _stockTickerObservable.StreamBacktests().AsChannelReader(10);
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
