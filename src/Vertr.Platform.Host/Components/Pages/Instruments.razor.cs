using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.Platform.Host.Components.Common;
using Vertr.Platform.Host.Components.Models;

namespace Vertr.Platform.Host.Components.Pages;

public partial class Instruments
{
    private bool _trapFocus = true;
    private bool _modal = true;

    private string message = string.Empty;

    private PaginationState pagination = new PaginationState() { ItemsPerPage = 2 };

    private IQueryable<SimplePerson> people = new[]
    {
        new SimplePerson(10895, "Jean", "Martin", 41),
        new SimplePerson(10944, "António", "Langa", 24),
        new SimplePerson(11203, "Julie", "Smith", 36),
        new SimplePerson(11205, "Nur", "Sari", 54),
        new SimplePerson(11898, "Jose", "Hernandez", 32),
        new SimplePerson(12130, "Kenji", "Sato", 18),
    }.AsQueryable();

    private IDialogReference? _dialog;

    private void Bonus(SimplePerson p) => message = $"You want to give {p.FirstName} {p.LastName} a regular bonus";

    private void DoubleBonus(SimplePerson p) => message = $"You want to give {p.FirstName} {p.LastName} a double bonus";

    private async Task HandleRowClick(FluentDataGridRow<SimplePerson> row)
    {
        // await OpenPanelRightAsync(row.Item);
        DemoLogger.WriteLine($"Row clicked: {row.RowIndex}");
    }

    private void HandleRowFocus(FluentDataGridRow<SimplePerson> row)
    {
        DemoLogger.WriteLine($"Row focused: {row.RowIndex}");
    }

    private void HandleCellClick(FluentDataGridCell<SimplePerson> cell)
    {
        DemoLogger.WriteLine($"Cell clicked: {cell.GridColumn}");
    }

    private void HandleCellFocus(FluentDataGridCell<SimplePerson> cell)
    {
        DemoLogger.WriteLine($"Cell focused : {cell.GridColumn}");
    }

    private async Task OpenPanelRightAsync(SimplePerson person)
    {
        DemoLogger.WriteLine($"Open right panel");

        _dialog = await DialogService.ShowPanelAsync<SimplePanel>(person, new DialogParameters<SimplePerson>()
        {
            Content = person,
            Alignment = HorizontalAlignment.Right,
            Title = $"Hello {person.FirstName}",
            PrimaryAction = "Close",
            SecondaryAction = null,
            Width = "500px"
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
            var simplePerson = result.Data as SimplePerson;
            DemoLogger.WriteLine($"Panel closed by {simplePerson?.FirstName} {simplePerson?.LastName} ({simplePerson?.Age})");
            return;
        }
    }

    private async Task OpenDialogAsync()
    {
        DemoLogger.WriteLine($"Open dialog centered");

        var simplePerson = people.First();

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

        IDialogReference dialog = await DialogService.ShowDialogAsync<InstrumentScreener>(simplePerson, parameters);
        DialogResult? result = await dialog.Result;


        if (result.Data is not null)
        {
            simplePerson = result.Data as SimplePerson;
            DemoLogger.WriteLine($"Dialog closed by {simplePerson?.FirstName} {simplePerson?.LastName} ({simplePerson?.Age}) - Canceled: {result.Cancelled}");
        }
        else
        {
            DemoLogger.WriteLine($"Dialog closed - Canceled: {result.Cancelled}");
        }
    }
}
