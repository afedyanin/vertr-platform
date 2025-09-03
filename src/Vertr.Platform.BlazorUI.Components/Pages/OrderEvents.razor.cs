using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.MarketData.Contracts;
using Vertr.OrderExecution.Contracts;
using Vertr.Platform.BlazorUI.Components.Common;
using Vertr.Platform.BlazorUI.Components.Models;
using Vertr.Platform.Common.Utils;

namespace Vertr.Platform.BlazorUI.Components.Pages;

public partial class OrderEvents
{
    private IDialogReference? _dialog;

    private PaginationState _pagination = new PaginationState() { ItemsPerPage = 12 };

    private FluentDataGrid<OrderEventModel> dataGrid;

    private IQueryable<OrderEventModel> _orderEvents { get; set; }

    private IDictionary<Guid, Instrument> _instruments { get; set; }

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _instruments = await InitInstruments();
        _orderEvents = await InitOrderEvents();
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

    private async Task<IQueryable<OrderEventModel>> InitOrderEvents()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
        var orderEvents = await apiClient.GetFromJsonAsync<OrderEvent[]>("api/order-events", JsonOptions.DefaultOptions);

        if (orderEvents == null)
        {
            return Array.Empty<OrderEventModel>().AsQueryable();
        }

        var modelItems = new List<OrderEventModel>();

        foreach (var orderEvent in orderEvents)
        {
            if (_instruments.TryGetValue(orderEvent.InstrumentId, out var instrument))
            {
                var item = new OrderEventModel
                {
                    OrderEvent = orderEvent,
                    Instrument = instrument,
                };

                modelItems.Add(item);
            }
        }

        var res = modelItems?.AsQueryable() ?? Array.Empty<OrderEventModel>().AsQueryable();
        return res;
    }
}
