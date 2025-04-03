using Microsoft.AspNetCore.SignalR;
using Vertr.Terminal.Shared.Models;
using Vertr.Terminal.Shared.Services;

namespace Vertr.Terminal.Server.Hubs;

public static class UserHandler
{
    public static HashSet<string> ConnectedIds = new HashSet<string>();
}

public class QuoteHub : Hub<IQuoteHub>
{

    private readonly ILogger<QuoteHub> _logger;

    public QuoteHub(ILogger<QuoteHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        UserHandler.ConnectedIds.Add(Context.ConnectionId);

        var msg = $"{Context.ConnectionId} joined the hub (Instance:{GetHashCode()})  clients-cnt:{UserHandler.ConnectedIds.Count}";

        _logger.LogInformation(msg);

        await Clients.All.QuoteHubMessage(msg);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        UserHandler.ConnectedIds.Remove(Context.ConnectionId);

        var msg = $"{Context.ConnectionId} left the hub (Instance:{GetHashCode()})  clients-cnt:{UserHandler.ConnectedIds.Count}";

        _logger.LogInformation(msg);

        await Clients.All.QuoteHubMessage(msg);

        await base.OnDisconnectedAsync(exception);
    }

    public Task QuoteHubMessage(string msg)
    {
        return Clients.All.QuoteHubMessage(msg);
    }

    public Task SendQuote(StockQuote quote)
    {
        return Clients.All.SendQuoteInfo(quote);
    }
}
