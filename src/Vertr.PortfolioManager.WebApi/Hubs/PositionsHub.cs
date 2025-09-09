using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Vertr.Platform.Common.Extensions;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.PortfolioManager.WebApi.Hubs;

public class PositionsHub : Hub
{
    private readonly IPositionObservable _positionObservable;

    public PositionsHub(
        IPositionObservable positionObservable)
    {
        _positionObservable = positionObservable;
    }

    public ChannelReader<Position> StreamPositions()
    {
        return _positionObservable.StreamPositions().AsChannelReader(10);
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
