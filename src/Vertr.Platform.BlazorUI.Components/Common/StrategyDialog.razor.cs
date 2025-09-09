using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Vertr.MarketData.Contracts;
using Vertr.Platform.BlazorUI.Components.Models;
using Vertr.Platform.Common.Utils;

namespace Vertr.Platform.BlazorUI.Components.Common;

public partial class StrategyDialog
{
    [Parameter]
    public StrategyModel Content { get; set; } = default!;

    private Instrument[] AllInstruments;

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
        AllInstruments = await InitInstruments(apiClient);

        await base.OnInitializedAsync();
    }

    private async Task<Instrument[]> InitInstruments(HttpClient apiClient)
        => await apiClient.GetFromJsonAsync<Instrument[]>("api/instruments", JsonOptions.DefaultOptions) ?? [];
}
