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

    private bool ActivateDiasbled => Content!.Strategy.IsActive;

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
        AllInstruments = await InitInstruments(apiClient);
        Content = await InitStrategy(apiClient);

        await base.OnInitializedAsync();
    }

    private async Task OnActivate()
    {
        if (Content?.Strategy == null)
        {
            return;
        }

        Content.Strategy.IsActive = true;

        var saved = await SaveStrategy(Content.Strategy);

        if (saved)
        {
            ToastService.ShowSuccess("Strategy activated!");
        }
    }
    private async Task OnDeactivate()
    {
        if (Content?.Strategy == null)
        {
            return;
        }

        Content.Strategy.IsActive = false;

        var saved = await SaveStrategy(Content.Strategy);

        if (saved)
        {
            ToastService.ShowSuccess("Strategy deactivated!");
        }
    }

    private async Task OnSave()
    {
        if (Content?.Strategy == null)
        {
            return;
        }

        var saved = await SaveStrategy(Content.Strategy);

        if (saved)
        {
            ToastService.ShowSuccess("Strategy saved!");
            StateHasChanged();
        }
    }

    private async Task<bool> SaveStrategy(StrategyMetadata metadata)
    {
        try
        {
            using var apiClient = _httpClientFactory.CreateClient("backend");
            var content = JsonContent.Create(metadata);
            var message = await apiClient.PostAsync("api/strategies", content);
            message.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"Cannot save strategy. Error: {ex.Message}");
            return false;
        }
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

        ToastService.ShowWarning($"Strategy {Content.Strategy.Name} is deleted.");

        Navigation.NavigateTo("/strategies");

        return;
    }
}

