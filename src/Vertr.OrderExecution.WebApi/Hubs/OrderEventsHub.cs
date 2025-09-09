using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.Platform.Common.Extensions;

namespace Vertr.OrderExecution.WebApi.Hubs;

public class OrderEventsHub : Hub
{
    private readonly IOrderEventObservable _orderEventsObservable;

    public OrderEventsHub(
        IOrderEventObservable orderEventsObservable)
    {
        _orderEventsObservable = orderEventsObservable;
    }

    public ChannelReader<OrderEvent> StreamOrderEvents()
    {
        return _orderEventsObservable.StreamOrderEvents().AsChannelReader(10);
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
