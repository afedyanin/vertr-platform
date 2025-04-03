using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Terminal.Shared.Models;

namespace Vertr.Terminal.Components.Pages;

public partial class StockTicker : IAsyncDisposable
{
    private HubConnection? _hubConnection;

    [Inject]
    protected NavigationManager Navigation { get; set; }

    public bool IsConnected =>
        _hubConnection?.State == HubConnectionState.Connected;

    private List<Stock> quotes = new List<Stock>();

    protected override async Task OnInitializedAsync()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(Navigation.ToAbsoluteUri("/hubs/stocks"))
            .AddMessagePackProtocol()
            .Build();

        await _hubConnection.StartAsync();
    }

    private async Task StartStreaming(CancellationToken cancellationToken)
    {
        if (!IsConnected)
        {
            return;
        }

        // https://learn.microsoft.com/en-us/aspnet/core/signalr/streaming?view=aspnetcore-9.0
        var stream = _hubConnection?.StreamAsync<Stock>("StreamStocks", cancellationToken);

        if (stream == null)
        {
            return;
        }

        await foreach (var stock in stream)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int i = quotes.FindIndex(q => q.Symbol == stock.Symbol);

            if (i < 0)
            {
                quotes.Add(stock);
                StateHasChanged();
            }
            else
            {
                // quotes[i].UpdateQuoteData(stock);

                //StateHasChanged();
            }

        }

    }

    private async Task OpenMarket()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.SendAsync("OpenMarket");
        }
    }

    private async Task CloseMarket()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.SendAsync("OpenMarket");
        }
    }

    private async Task Reset()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.SendAsync("Reset");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
