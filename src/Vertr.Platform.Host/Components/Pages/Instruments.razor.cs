using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.MarketData.Contracts;
using Vertr.Platform.Host.Components.Common;

namespace Vertr.Platform.Host.Components.Pages;

public partial class Instruments
{
    private IDialogReference? _dialog;

    private PaginationState _pagination = new PaginationState() { ItemsPerPage = 2 };

    private IQueryable<Instrument> _instrumentList { get; set; }

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        using var apiClient = _httpClientFactory.CreateClient("backend");
        var items = await apiClient.GetFromJsonAsync<Instrument[]>("api/instruments");
        _instrumentList = items?.AsQueryable() ?? Array.Empty<Instrument>().AsQueryable();
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
        // DemoLogger.WriteLine($"Open dialog centered");

        var selected = _instrumentList.First();

        DialogParameters parameters = new()
        {
            Title = "Search instrument",
            PrimaryAction = "Select",
            SecondaryAction = null,
            Width = "500px",
            TrapFocus = true,
            Modal = true,
            PreventScroll = true
        };

        IDialogReference dialog = await DialogService.ShowDialogAsync<InstrumentScreener>(selected, parameters);
        DialogResult? result = await dialog.Result;


        if (result.Data is not null)
        {
            selected = result.Data as Instrument;
            //DemoLogger.WriteLine($"Dialog closed by {selected?.Name} Canceled: {result.Cancelled}");
        }
        else
        {
            //DemoLogger.WriteLine($"Dialog closed - Canceled: {result.Cancelled}");
        }
    }
}
