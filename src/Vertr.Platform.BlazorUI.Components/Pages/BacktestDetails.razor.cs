using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Vertr.Backtest.Contracts;
using Vertr.Platform.BlazorUI.Components.Models;
using Vertr.Platform.Common.Utils;
using Vertr.PortfolioManager.Contracts;
using Vertr.Strategies.Contracts;

namespace Vertr.Platform.BlazorUI.Components.Pages;
public partial class BacktestDetails
{
    private IDictionary<Guid, StrategyMetadata> _strategies { get; set; }

    private IDictionary<Guid, Portfolio> _portfolios { get; set; }

    public BacktestModel? Content { get; set; } = default!;

    [Parameter]
    public string? BacktestId { get; set; }

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");

        _portfolios = await InitPortfolios(apiClient);
        _strategies = await InitStrategies(apiClient);

        Content = await InitBacktest(apiClient);
        await base.OnInitializedAsync();
    }

    private bool CancelDiasbled => Content!.Backtest.ExecutionState
        is not ExecutionState.InProgress
        || Content.Backtest.IsCancellationRequested;
    private bool StartDiasbled => Content!.Backtest.ExecutionState
        is not ExecutionState.Created
        and not ExecutionState.Enqueued;

    private async Task OnCancelAsync()
    {
        try
        {
            using var apiClient = _httpClientFactory.CreateClient("backend");
            var message = await apiClient.PutAsync($"api/backtests/cancel/{BacktestId}", null);
            message.EnsureSuccessStatusCode();
        }
        catch
        {
        }
    }

    private async Task OnStartAsync()
    {
        try
        {
            using var apiClient = _httpClientFactory.CreateClient("backend");
            var message = await apiClient.PutAsync($"api/backtests/start/{BacktestId}", null);
            message.EnsureSuccessStatusCode();
        }
        catch
        {
        }
    }

    private async Task<BacktestModel?> InitBacktest(HttpClient apiClient)
    {
        var backtest = await apiClient.GetFromJsonAsync<BacktestRun>($"api/backtests/{BacktestId}", JsonOptions.DefaultOptions);

        if (backtest == null)
        {
            return null;
        }

        if (!_strategies.TryGetValue(backtest.StrategyId, out var strategy))
        {
            return null;
        }

        if (!_portfolios.TryGetValue(backtest.PortfolioId, out var portfolio))
        {
            return null;
        }

        var model = new BacktestModel
        {
            Backtest = backtest,
            Strategy = strategy,
            Portfolio = portfolio
        };

        return model;
    }

    private async Task<IDictionary<Guid, StrategyMetadata>> InitStrategies(HttpClient apiClient)
    {
        var strategies = await apiClient.GetFromJsonAsync<StrategyMetadata[]>("api/strategies", JsonOptions.DefaultOptions);
        var res = new Dictionary<Guid, StrategyMetadata>();

        if (strategies == null)
        {
            return res;
        }

        foreach (var strategy in strategies)
        {
            res[strategy.Id] = strategy;
        }

        return res;
    }

    private async Task<IDictionary<Guid, Portfolio>> InitPortfolios(HttpClient apiClient)
    {
        var portfolios = await apiClient.GetFromJsonAsync<Portfolio[]>("api/portfolios", JsonOptions.DefaultOptions);
        var res = new Dictionary<Guid, Portfolio>();

        if (portfolios == null)
        {
            return res;
        }

        foreach (var portfolio in portfolios)
        {
            res[portfolio.Id] = portfolio;
        }

        return res;
    }

    private async Task HandleDeleteAction()
    {
        var backtest = Content!.Backtest;

        var confirmation = await DialogService.ShowConfirmationAsync(
            $"Delete backtest: {backtest.Description}?",
            "Yes",
            "No",
            $"Deleting backtest {backtest.Description}");

        var result = await confirmation.Result;

        if (result.Cancelled)
        {
            return;
        }

        using var apiClient = _httpClientFactory.CreateClient("backend");

        // delete portfolio
        var portfolioDeleted = await apiClient.DeleteAsync($"api/portfolios/{backtest.PortfolioId}");
        portfolioDeleted.EnsureSuccessStatusCode();

        // delete trading operations
        var operationsDeleted = await apiClient.DeleteAsync($"api/trade-operations/{backtest.PortfolioId}");
        operationsDeleted.EnsureSuccessStatusCode();

        // delete order events
        var ordersDeleted = await apiClient.DeleteAsync($"api/order-events/{backtest.PortfolioId}");
        ordersDeleted.EnsureSuccessStatusCode();

        // delete backtest
        var message = await apiClient.DeleteAsync($"api/backtests/{backtest.Id}");
        message.EnsureSuccessStatusCode();

        ToastService.ShowWarning($"Backtest {backtest.Description} is deleted.");

        Navigation.NavigateTo("/backtests");
    }
}
