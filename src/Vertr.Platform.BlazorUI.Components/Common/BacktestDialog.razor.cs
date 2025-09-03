using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

using Vertr.Platform.BlazorUI.Components.Models;
using Vertr.Platform.Common.Utils;
using Vertr.Strategies.Contracts;

namespace Vertr.Platform.BlazorUI.Components.Common;

public partial class BacktestDialog
{
    [Parameter]
    public BacktestModel Content { get; set; } = default!;

    private StrategyMetadata[] AllStrategies;

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
        AllStrategies = await InitStrategies(apiClient);
        await base.OnInitializedAsync();
    }

    private async Task<StrategyMetadata[]> InitStrategies(HttpClient apiClient)
        => await apiClient.GetFromJsonAsync<StrategyMetadata[]>("api/strategies", JsonOptions.DefaultOptions) ?? [];
}
