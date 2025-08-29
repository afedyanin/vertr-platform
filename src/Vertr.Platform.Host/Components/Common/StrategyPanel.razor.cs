using Microsoft.AspNetCore.Components;
using Vertr.MarketData.Contracts;
using Vertr.Platform.Common.Utils;
using Vertr.Platform.Host.Components.Models;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.Platform.Host.Components.Common;

public partial class StrategyPanel
{
    [Parameter]
    public StrategyModel Content { get; set; } = default!;

    private Instrument[] AllInstruments;

    private Portfolio[] AllPortfolios;

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
        AllInstruments = await InitInstruments(apiClient);
        AllPortfolios = await InitPortfolios(apiClient);

        await base.OnInitializedAsync();
    }

    private async Task<Instrument[]> InitInstruments(HttpClient apiClient)
        => await apiClient.GetFromJsonAsync<Instrument[]>("api/instruments", JsonOptions.DefaultOptions) ?? [];

    private async Task<Portfolio[]> InitPortfolios(HttpClient apiClient)
        => await apiClient.GetFromJsonAsync<Portfolio[]>("api/portfolios", JsonOptions.DefaultOptions) ?? [];

}
