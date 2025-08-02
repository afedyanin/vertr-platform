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
        //DemoLogger.WriteLine($"Row clicked: {row.RowIndex}");
    }

    private void HandleRowFocus(FluentDataGridRow<Instrument> row)
    {
        //DemoLogger.WriteLine($"Row focused: {row.RowIndex}");
    }

    private async Task HandleCellClick(FluentDataGridCell<Instrument> cell)
    {
        if (cell.Item != null && cell.GridColumn <= 6)
        {
            await OpenPanelRightAsync(cell.Item);
        }

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

        if (result.Data is null)
        {
            return;
        }

        var selectedInstrument = result.Data as Instrument;
        var instrumentId = selectedInstrument?.Id;

        if (instrumentId == null)
        {
            return;
        }

        await AddInstrument(instrumentId.Value);
        await dataGrid.RefreshDataAsync(force: true);
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
        var instrument = await apiClient.GetFromJsonAsync<Instrument>($"api/tinvest/instrument-by-id/{instrumentId}");

        if (instrument == null)
        {
            return;
        }

        DemoLogger.WriteLine($"Adding instrument: {instrument.Symbol.ClassCode}.{instrument.Symbol.Ticker} ({instrument.Name})");

        var content = JsonContent.Create(instrument);
        await apiClient.PostAsync("api/instruments", content);
        _instrumentList = await InitInstruments();
    }
    private async Task HandleDeleteAction(Instrument instrument)
    {
        var confirmation = await DialogService.ShowConfirmationAsync(
            $"Delete instrument: {instrument.Name}?",
            "Yes",
            "No",
            $"Deleteing {instrument.Symbol.ClassCode}.{instrument.Symbol.Ticker}");

        var result = await confirmation.Result;

        if (result.Cancelled)
        {
            return;
        }

        DemoLogger.WriteLine($"Deleting instrument: {instrument.Symbol.ClassCode}.{instrument.Symbol.Ticker} ({instrument.Name})");

        using var apiClient = _httpClientFactory.CreateClient("backend");
        await apiClient.DeleteAsync($"api/instruments/{instrument.Id}");
        _instrumentList = await InitInstruments();
        await dataGrid.RefreshDataAsync(force: true);
    }
}
