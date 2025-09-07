using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.Platform.BlazorUI.Components.Models;

namespace Vertr.Platform.BlazorUI.Components.Pages;

public partial class StockTicker : IAsyncDisposable
{
    private FluentDataGrid<StockModel> dataGrid;

    private HubConnection _hubConnection;

    private bool _isConnected =>
           _hubConnection?.State == HubConnectionState.Connected;

    private Dictionary<string, StockModel> _stocksDict = [];

    private IQueryable<StockModel> _stocks => _stocksDict.Values.AsQueryable();

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(Navigation.ToAbsoluteUri("/stocksHub"))
            .Build();

        await _hubConnection.StartAsync();
        _stocksDict = await InitStocks();

        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await StartStreaming();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task<Dictionary<string, StockModel>> InitStocks()
    {
        var res = new Dictionary<string, StockModel>();

        var items = await _hubConnection.InvokeAsync<StockModel[]>("GetSnapshot");

        foreach (var item in items)
        {
            res[item.Symbol] = item;
            item.Updated = ModelUpdated;
        }

        return res;
    }
    private async Task StartStreaming()
    {
        var stream = _hubConnection.StreamAsync<StockModel>("StreamStocks");

        await foreach (var item in stream)
        {
            _stocksDict[item.Symbol] = item;
            item.Updated = ModelUpdated;
            StateHasChanged();
        }
    }

    private void OnCellFocused(FluentDataGridCell<StockModel> cell)
    {
        if (cell.GridColumn == 7)
        {
            var castedField = cell.ChildContent?.Target as FluentNumberField<decimal>;
            Console.WriteLine($"Casted field value: {castedField?.Value}");
        }
    }

    private async Task ModelUpdated(StockModel model)
    {
        await _hubConnection.SendAsync("OnNewLastChangeInput", model);
    }
}
