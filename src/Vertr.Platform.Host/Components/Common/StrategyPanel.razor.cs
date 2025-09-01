using Microsoft.AspNetCore.Components;
using Vertr.MarketData.Contracts;
using Vertr.Platform.Common.Utils;
using Vertr.Platform.Host.Components.Models;

namespace Vertr.Platform.Host.Components.Common;

public partial class StrategyPanel
{
    [Parameter]
    public StrategyModel Content { get; set; } = default!;

    private Instrument[] AllInstruments;

    private string _portfolioDeatilsLink => $"/portfolios/details/{Content?.Strategy.PortfolioId}";

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
