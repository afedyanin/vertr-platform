using Microsoft.AspNetCore.Components;
using Vertr.Platform.Common.Utils;
using Vertr.Platform.Host.Components.Models;
using Vertr.Strategies.Contracts;

namespace Vertr.Platform.Host.Components.Common;

public partial class BacktestDialog
{
    [Parameter]
    public BacktestModel Content { get; set; } = default!;

    private StrategyMetadata[] AllStrategies;

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        AllStrategies = await InitStrategies();
        await base.OnInitializedAsync();
    }

    private async Task<StrategyMetadata[]> InitStrategies()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
        var strategies = await apiClient.GetFromJsonAsync<StrategyMetadata[]>("api/strategies", JsonOptions.DefaultOptions);

        return strategies ?? [];
    }
}
