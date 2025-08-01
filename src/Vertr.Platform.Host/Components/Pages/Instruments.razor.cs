using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.MarketData.Contracts;
using Vertr.Platform.Host.Components.Common;

namespace Vertr.Platform.Host.Components.Pages;

public partial class Instruments
{
    private IDialogReference? _dialog;

    private PaginationState _pagination = new PaginationState() { ItemsPerPage = 12 };

    private IQueryable<Instrument> _instrumentList { get; set; }

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    private FluentDataGrid<Instrument> dataGrid;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _instrumentList = await InitInstruments();
    }

    private async Task HandleRowClick(FluentDataGridRow<Instrument> row)
    {
        await OpenPanelRightAsync(row.Item);
        //DemoLogger.WriteLine($"Row clicked: {row.RowIndex}");
    }

    private void HandleRowFocus(FluentDataGridRow<Instrument> row)
    {
        //DemoLogger.WriteLine($"Row focused: {row.RowIndex}");
    }

    private void HandleCellClick(FluentDataGridCell<Instrument> cell)
    {
        //DemoLogger.WriteLine($"Cell clicked: {cell.GridColumn}");
    }

    private void HandleCellFocus(FluentDataGridCell<Instrument> cell)
    {
        //DemoLogger.WriteLine($"Cell focused : {cell.GridColumn}");
    }

    private async Task OpenPanelRightAsync(Instrument instrument)
    {
        // DemoLogger.WriteLine($"Open right panel");

        _dialog = await DialogService.ShowPanelAsync<InstrumentPanel>(instrument, new DialogParameters<Instrument>()
        {
            Content = instrument,
            Alignment = HorizontalAlignment.Right,
            Title = $"{instrument.Name}",
            PrimaryAction = "Close",
            SecondaryAction = null,
            Width = "400px",
        });
        DialogResult result = await _dialog.Result;
        HandlePanel(result);
    }
    private static void HandlePanel(DialogResult result)
    {
        if (result.Cancelled)
        {
            // DemoLogger.WriteLine($"Panel cancelled");
            return;
        }

        if (result.Data is not null)
        {
            var instrument = result.Data as Instrument;
            // DemoLogger.WriteLine($"Panel closed by {instrument?.Name}");
            return;
        }
    }

    private async Task OpenDialogAsync()
    {
        var dummyInstrument = new Instrument
        {
            Symbol = new Symbol("", "")
        };

        var parameters = new DialogParameters()
        {
            Title = "Search instrument",
            PrimaryAction = "Select",
            SecondaryAction = null,
            Width = "500px",
            TrapFocus = true,
            Modal = true,
            PreventScroll = true
        };

        var dialog = await DialogService.ShowDialogAsync<InstrumentScreener>(dummyInstrument, parameters);
        var result = await dialog.Result;

        if (result.Data is not null)
        {
            var selectedInstrument = result.Data as Instrument;
            var instrumentId = selectedInstrument?.Id;

            if (instrumentId != null)
            {
                DemoLogger.WriteLine($"Selected instrumentId={instrumentId}");
                await AddInstrument(instrumentId.Value);
                // TODO: Refresh grid
            }
        }
        else
        {
            // DemoLogger.WriteLine($"Dialog closed - Canceled: {result.Cancelled}");
        }
    }

    private async Task<IQueryable<Instrument>> InitInstruments()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
        var items = await apiClient.GetFromJsonAsync<Instrument[]>("api/instruments");
        var res = items?.AsQueryable() ?? Array.Empty<Instrument>().AsQueryable();
        return res;
    }

    private async Task AddInstrument(Guid instrumentId)
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
        var item = await apiClient.GetFromJsonAsync<Instrument>($"api/tinvest/instrument-by-id/{instrumentId}");

        if (item == null)
        {
            return;
        }
        var content = JsonContent.Create(item);
        await apiClient.PostAsync("api/instruments", content);
        _instrumentList = await InitInstruments();
    }
}
