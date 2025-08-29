using Microsoft.AspNetCore.Components;
using Vertr.Platform.Common.Utils;
using Vertr.Platform.Host.Components.Models;
using Vertr.PortfolioManager.Contracts;
using Vertr.Strategies.Contracts;

namespace Vertr.Platform.Host.Components.Common;

public partial class BacktestDialog
{
    [Parameter]
    public BacktestModel Content { get; set; } = default!;

    private StrategyMetadata[] AllStrategies;

    private Portfolio[] AllPortfolios;

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
        AllStrategies = await InitStrategies(apiClient);
        AllPortfolios = await InitPortfolios(apiClient);

        await base.OnInitializedAsync();
    }

    private async Task<StrategyMetadata[]> InitStrategies(HttpClient apiClient)
        => await apiClient.GetFromJsonAsync<StrategyMetadata[]>("api/strategies", JsonOptions.DefaultOptions) ?? [];

    private async Task<Portfolio[]> InitPortfolios(HttpClient apiClient)
        => await apiClient.GetFromJsonAsync<Portfolio[]>("api/portfolios", JsonOptions.DefaultOptions) ?? [];
}
