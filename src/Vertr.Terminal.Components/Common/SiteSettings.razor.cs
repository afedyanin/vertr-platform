using Microsoft.FluentUI.AspNetCore.Components;

namespace Vertr.Terminal.Components.Common;

public partial class SiteSettings
{
    private IDialogReference? _dialog;

    private async Task OpenSiteSettingsAsync()
    {
        DemoLogger.WriteLine($"Open site settings");
        _dialog = await DialogService.ShowPanelAsync<SiteSettingsPanel>(new DialogParameters()
        {
            ShowTitle = true,
            Title = "Site settings",
            Alignment = HorizontalAlignment.Right,
            PrimaryAction = "OK",
            SecondaryAction = null,
            ShowDismiss = true
        });

        var result = await _dialog.Result;
    }
}
