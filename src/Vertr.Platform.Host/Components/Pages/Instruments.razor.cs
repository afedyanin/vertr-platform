using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.MarketData.Contracts;
using Vertr.Platform.Host.Components.Common;

namespace Vertr.Platform.Host.Components.Pages;

public partial class Instruments
{
    private bool _trapFocus = true;
    private bool _modal = true;

    private string message = string.Empty;

    private PaginationState pagination = new PaginationState() { ItemsPerPage = 2 };

    private IQueryable<Instrument> instruments = new[]
    {
        new Instrument
        {
            Id = Guid.NewGuid(),
            Symbol = new Symbol("CCC", "TTT"),
            Currency = "rub",
            Name = "Test instrument",
            LotSize = 1,
        },
        new Instrument
        {
            Id = Guid.NewGuid(),
            Symbol = new Symbol("AAA", "LLL"),
            Currency = "rub",
            Name = "Test instrument",
            LotSize = 1,
        },
        new Instrument
        {
            Id = Guid.NewGuid(),
            Symbol = new Symbol("CCC", "RRRR"),
            Currency = "rub",
            Name = "Test instrument",
            LotSize = 1,
        },
        new Instrument
        {
            Id = Guid.NewGuid(),
            Symbol = new Symbol("BBB", "REW"),
            Currency = "rub",
            Name = "Test instrument",
            LotSize = 1,
        },
        new Instrument
        {
            Id = Guid.NewGuid(),
            Symbol = new Symbol("EEE", "TTT"),
            Currency = "rub",
            Name = "Test instrument",
            LotSize = 1,
        },

    }.AsQueryable();

    private IDialogReference? _dialog;

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
        DemoLogger.WriteLine($"Open right panel");

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
            DemoLogger.WriteLine($"Panel cancelled");
            return;
        }

        if (result.Data is not null)
        {
            var instrument = result.Data as Instrument;
            DemoLogger.WriteLine($"Panel closed by {instrument?.Name}");
            return;
        }
    }

    private async Task OpenDialogAsync()
    {
        DemoLogger.WriteLine($"Open dialog centered");

        var selected = instruments.First();

        DialogParameters parameters = new()
        {
            Title = "Search instrument",
            PrimaryAction = "Select",
            SecondaryAction = null,
            Width = "500px",
            TrapFocus = _trapFocus,
            Modal = _modal,
            PreventScroll = true
        };

        IDialogReference dialog = await DialogService.ShowDialogAsync<InstrumentScreener>(selected, parameters);
        DialogResult? result = await dialog.Result;


        if (result.Data is not null)
        {
            selected = result.Data as Instrument;
            DemoLogger.WriteLine($"Dialog closed by {selected?.Name} Canceled: {result.Cancelled}");
        }
        else
        {
            DemoLogger.WriteLine($"Dialog closed - Canceled: {result.Cancelled}");
        }
    }
}
