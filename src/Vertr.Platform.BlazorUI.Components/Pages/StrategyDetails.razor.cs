using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Vertr.MarketData.Contracts;
using Vertr.Platform.BlazorUI.Components.Common;
using Vertr.Platform.BlazorUI.Components.Models;
using Vertr.Platform.Common.Utils;
using Vertr.Strategies.Contracts;

namespace Vertr.Platform.BlazorUI.Components.Pages;
public partial class StrategyDetails
{
    public StrategyModel? Content { get; set; } = default!;

    [Parameter]
    public string? StrategyId { get; set; }

    private Instrument[] AllInstruments = [];

    private string _portfolioDetailsLink => $"/portfolios/details/{Content?.Strategy.PortfolioId}";

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
        AllInstruments = await InitInstruments(apiClient);
        Content = await InitStrategy(apiClient);

        await base.OnInitializedAsync();
    }

    private async Task<Instrument[]> InitInstruments(HttpClient apiClient)
        => await apiClient.GetFromJsonAsync<Instrument[]>("api/instruments", JsonOptions.DefaultOptions) ?? [];

    private async Task<StrategyModel?> InitStrategy(HttpClient apiClient)
    {
        var strategy = await apiClient.GetFromJsonAsync<StrategyMetadata>($"api/strategies/{StrategyId}", JsonOptions.DefaultOptions);

        if (strategy == null)
        {
            return null;
        }

        var instrument = AllInstruments.FirstOrDefault(i => i.Id == strategy.InstrumentId);

        if (instrument == null)
        {
            return null;
        }

        var model = new StrategyModel
        {
            Strategy = strategy,
            Instrument = instrument,
        };

        return model;
    }

    private async Task HandleDeleteAction()
    {
        var confirmation = await DialogService.ShowConfirmationAsync(
            $"Delete strategy: {Content!.Strategy.Name}?",
            "Yes",
            "No",
            $"Deleting {Content!.Strategy.Name}");

        var result = await confirmation.Result;

        if (result.Cancelled)
        {
            return;
        }

        using var apiClient = _httpClientFactory.CreateClient("backend");

        // delete portfolio
        var portfolioDeleted = await apiClient.DeleteAsync($"api/portfolios/{Content.Strategy.PortfolioId}");
        portfolioDeleted.EnsureSuccessStatusCode();

        // delete trading operations
        var operationsDeleted = await apiClient.DeleteAsync($"api/trade-operations/{Content.Strategy.PortfolioId}");
        operationsDeleted.EnsureSuccessStatusCode();

        // delete order events
        var ordersDeleted = await apiClient.DeleteAsync($"api/order-events/{Content.Strategy.PortfolioId}");
        ordersDeleted.EnsureSuccessStatusCode();

        // delete trading signals
        var signalsDeleted = await apiClient.DeleteAsync($"api/trading-signals/by-strategy/{Content.Strategy.Id}");
        signalsDeleted.EnsureSuccessStatusCode();

        var message = await apiClient.DeleteAsync($"api/strategies/{Content.Strategy.Id}");
        message.EnsureSuccessStatusCode();

        DemoLogger.WriteLine($"Strategy {Content.Strategy.Name} is deleted.");

        Navigation.NavigateTo("/strategies");

        return;
    }

}

