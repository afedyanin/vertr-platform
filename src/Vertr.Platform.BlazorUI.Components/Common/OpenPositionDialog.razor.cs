using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Vertr.MarketData.Contracts;
using Vertr.Platform.BlazorUI.Components.Models;
using Vertr.Platform.Common.Utils;

namespace Vertr.Platform.BlazorUI.Components.Common;

public partial class OpenPositionDialog
{
    [Parameter]
    public OpenPositionModel Content { get; set; } = default!;

    private Instrument[] _instruments = [];

    private bool _selectDateDisabled => !Content.OrderExecutionSimulated;
    private bool _selectPriceDisabled => !Content.OrderExecutionSimulated;

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _instruments = await InitInstruments();
        await base.OnInitializedAsync();
    }

    private async Task<Instrument[]> InitInstruments()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
        var instruments = await apiClient.GetFromJsonAsync<Instrument[]>("api/instruments", JsonOptions.DefaultOptions);

        var filterd = instruments?
            .Where(x =>
                !string.IsNullOrEmpty(x.InstrumentType) &&
                !x.InstrumentType.Equals("currency", StringComparison.OrdinalIgnoreCase))
            .ToArray() ?? [];

        return filterd;
    }
}
