using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Vertr.MarketData.Contracts;
using Vertr.Platform.BlazorUI.Components.Models;
using Vertr.Platform.Common.Utils;

namespace Vertr.Platform.BlazorUI.Components.Common;

public partial class OrderEventPanel
{
    [Parameter]
    public OrderEventModel Content { get; set; } = default!;

    private Instrument[] AllInstruments;

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        AllInstruments = await InitInstruments();
        await base.OnInitializedAsync();
    }

    private async Task<Instrument[]> InitInstruments()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
        var instruments = await apiClient.GetFromJsonAsync<Instrument[]>("api/instruments", JsonOptions.DefaultOptions);

        return instruments ?? [];
    }

}
