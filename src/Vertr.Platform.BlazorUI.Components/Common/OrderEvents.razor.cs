using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components.Utilities;
using Microsoft.JSInterop;
using Vertr.MarketData.Contracts;
using Vertr.OrderExecution.Contracts;
using Vertr.Platform.BlazorUI.Components.Models;
using Vertr.Platform.Common.Utils;

namespace Vertr.Platform.BlazorUI.Components.Common;

public partial class OrderEvents : IAsyncDisposable
{
    private HubConnection _hubConnection;

    private bool _isConnected =>
           _hubConnection?.State == HubConnectionState.Connected;

    private IDialogReference? _dialog;

    private FluentDataGrid<OrderEventModel> dataGrid;

    private IQueryable<OrderEventModel> _orderEvents =>
        _orderEventList
        .OrderByDescending(e => e.OrderEvent.CreatedAt)
        .ThenBy(e => e.OrderEvent.RequestId)
        .Take(100)
        .AsQueryable();

    private List<OrderEventModel> _orderEventList = [];

    private IDictionary<Guid, Instrument> _instruments { get; set; }

    [Parameter]
    public string? PortfolioId { get; set; }

    [Parameter]
    public bool UseOrderEventStream { get; set; }

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    public async Task RefreshDataAsync()
    {
        _orderEventList = await InitOrderEvents();
        StateHasChanged();
        //await dataGrid.RefreshDataAsync(force: true);
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        _instruments = await InitInstruments();
        _orderEventList = await InitOrderEvents();

        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender && UseOrderEventStream)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(Navigation.ToAbsoluteUri("/orderEventsHub"))
                .Build();

            await _hubConnection.StartAsync();
            await StartStreaming();
        }
    }

    private async Task StartStreaming()
    {
        var stream = _hubConnection.StreamAsync<OrderEvent>("StreamOrderEvents");

        await foreach (var orderEvent in stream)
        {
            if (orderEvent.PortfolioId.ToString() != PortfolioId)
            {
                continue;
            }

            var model = CreateModel(orderEvent);
            if (model != null)
            {
                _orderEventList.Add(model);
                StateHasChanged();
                await dataGrid.RefreshDataAsync();
            }
        }
    }

    private async Task HandleCellClick(FluentDataGridCell<OrderEventModel> cell)
    {
        if (cell.Item != null)
        {
            await OpenPanelRightAsync(cell.Item);
        }
    }

    private async Task OpenPanelRightAsync(OrderEventModel orderEventModel)
    {
        _dialog = await DialogService.ShowPanelAsync<OrderEventPanel>(orderEventModel, new DialogParameters<OrderEventModel>()
        {
            Content = orderEventModel,
            Alignment = HorizontalAlignment.Right,
            Title = "Order event",
            PrimaryAction = "Close",
            SecondaryAction = null,
            Width = "400px",
        });

        _ = await _dialog.Result;
    }

    private async Task<IDictionary<Guid, Instrument>> InitInstruments()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
        var instruments = await apiClient.GetFromJsonAsync<Instrument[]>("api/instruments", JsonOptions.DefaultOptions);
        var res = new Dictionary<Guid, Instrument>();

        if (instruments == null)
        {
            return res;
        }

        foreach (var instrument in instruments)
        {
            res[instrument.Id] = instrument;
        }

        return res;
    }

    private async Task<List<OrderEventModel>> InitOrderEvents()
    {
        if (PortfolioId == null)
        {
            return [];
        }

        using var apiClient = _httpClientFactory.CreateClient("backend");

        var orderEvents = await apiClient.GetFromJsonAsync<OrderEvent[]>(
            $"api/order-events/by-portfolio/{PortfolioId}",
            JsonOptions.DefaultOptions);

        if (orderEvents == null)
        {
            return [];
        }

        var modelItems = new List<OrderEventModel>();

        foreach (var orderEvent in orderEvents)
        {
            var model = CreateModel(orderEvent);
            if (model != null)
            {
                modelItems.Add(model);
            }
        }

        return modelItems;
    }

    private OrderEventModel? CreateModel(OrderEvent orderEvent)
    {
        if (!_instruments.TryGetValue(orderEvent.InstrumentId, out var instrument))
        {
            return null;
        }

        var item = new OrderEventModel
        {
            OrderEvent = orderEvent,
            Instrument = instrument,
        };

        return item;
    }

    private string GetEventName(OrderEvent orderEvent)
    {
        var res = orderEvent.JsonDataType?.Split('.') ?? [];

        return res.Last();
    }
}
