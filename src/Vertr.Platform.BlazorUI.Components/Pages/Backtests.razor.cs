using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.Backtest.Contracts;
using Vertr.Backtest.Contracts.Commands;
using Vertr.Platform.BlazorUI.Components.Common;
using Vertr.Platform.BlazorUI.Components.Models;
using Vertr.Platform.Common.Utils;
using Vertr.PortfolioManager.Contracts;
using Vertr.Strategies.Contracts;

namespace Vertr.Platform.BlazorUI.Components.Pages;

public partial class Backtests : IAsyncDisposable
{
    private HubConnection _hubConnection;

    private bool _isConnected =>
           _hubConnection?.State == HubConnectionState.Connected;

    private IDictionary<Guid, BacktestModel> _backtestsDict = new Dictionary<Guid, BacktestModel>();

    private IQueryable<BacktestModel> _backtests => _backtestsDict.Values.AsQueryable();

    private IDictionary<Guid, StrategyMetadata> _strategies { get; set; }

    private IDictionary<Guid, Portfolio> _portfolios { get; set; }

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(Navigation.ToAbsoluteUri("/backtestsHub"))
            .Build();

        await _hubConnection.StartAsync();


        using var apiClient = _httpClientFactory.CreateClient("backend");

        _portfolios = await InitPortfolios(apiClient);
        _strategies = await InitStrategies(apiClient);
        _backtestsDict = await InitBacktests(apiClient);

        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await StartStreaming();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task StartStreaming()
    {
        var stream = _hubConnection.StreamAsync<BacktestRun>("StreamBacktestsProgress");

        await foreach (var backtest in stream)
        {
            var model = CreateModel(backtest);
            if (model != null)
            {
                _backtestsDict[backtest.Id] = model;
                StateHasChanged();
            }
        }
    }

    private void HandleCellClick(FluentDataGridCell<BacktestModel> cell)
    {
        if (cell.Item == null)
        {
            return;
        }

        Navigation.NavigateTo($"backtests/details/{cell.Item.Backtest.Id}");
    }

    private async Task OpenDialogAsync()
    {
        var strategy = _strategies.Values.First();

        var initialModel = new BacktestModel()
        {
            Strategy = strategy,
            Portfolio = null,
            Backtest = new BacktestRun
            {
                Id = Guid.NewGuid(),
                From = DateTime.UtcNow.AddDays(-10),
                To = DateTime.UtcNow,
                Description = "Some Test",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ExecutionState = ExecutionState.Created,
                IsCancellationRequested = false,
                ProgressMessage = string.Empty,
            }
        };

        var parameters = new DialogParameters()
        {
            Alignment = HorizontalAlignment.Right,
            Title = "Create new backtest",
            PrimaryAction = "Save",
            SecondaryAction = "Cancel",
            Width = "500px",
            TrapFocus = true,
            Modal = true,
            PreventScroll = true
        };

        var dialog = await DialogService.ShowDialogAsync<BacktestDialog>(initialModel, parameters);
        var result = await dialog.Result;

        if (result.Cancelled)
        {
            return;
        }

        if (result.Data is null)
        {
            return;
        }

        if (result.Data is not BacktestModel model)
        {
            return;
        }

        var btPortfolio = new Portfolio
        {
            Id = Guid.NewGuid(),
            IsBacktest = true,
            Name = $"{model.Backtest.Description}",
            UpdatedAt = model.Backtest.CreatedAt,
        };

        var portfolioSaved = await SavePortfolio(btPortfolio);

        if (!portfolioSaved)
        {
            ToastService.ShowError($"Cannot create portfolio for Backtest: {model.Backtest.Description}.");
            return;
        }

        model.Portfolio = btPortfolio;

        var saved = await SaveBacktest(model);

        if (!saved)
        {
            ToastService.ShowError($"Saving backtest {model.Backtest.Description} failed.");
            return;

        }

        ToastService.ShowSuccess($"Backtest {model.Backtest.Description} created.");

        Navigation.NavigateTo($"backtests/details/{model.Backtest.Id}");
    }


    private async Task<IDictionary<Guid, BacktestModel>> InitBacktests(HttpClient apiClient)
    {
        var backtests = await apiClient.GetFromJsonAsync<BacktestRun[]>("api/backtests", JsonOptions.DefaultOptions);

        var res = new Dictionary<Guid, BacktestModel>();

        if (backtests == null)
        {
            return res;
        }

        foreach (var backtest in backtests)
        {
            var model = CreateModel(backtest);

            if (model != null)
            {
                res[backtest.Id] = model;
            }
        }

        return res;
    }

    private BacktestModel? CreateModel(BacktestRun backtest)
    {
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

    private async Task<bool> SavePortfolio(Portfolio portfolio)
    {
        try
        {
            using var apiClient = _httpClientFactory.CreateClient("backend");
            var content = JsonContent.Create(portfolio);
            var message = await apiClient.PostAsync("api/portfolios", content);
            message.EnsureSuccessStatusCode();
            return true;
        }
        catch
        {
            // TODO: Use toast service
            return false;
        }
    }

    private async Task<bool> SaveBacktest(BacktestModel backtestModel)
    {
        try
        {
            var request = new CreateBacktestRequest
            {
                BacktestId = backtestModel.Backtest.Id,
                Description = backtestModel.Backtest.Description,
                From = backtestModel.ComposeDateFrom(),
                To = backtestModel.ComposeDateTo(),
                StrategyId = backtestModel.Strategy.Id,
                PortfolioId = backtestModel.Portfolio!.Id,
                StartImmediately = backtestModel.StartImmediately,
            };

            using var apiClient = _httpClientFactory.CreateClient("backend");
            var content = JsonContent.Create(request);
            var message = await apiClient.PostAsync("api/backtests", content);
            message.EnsureSuccessStatusCode();

            return true;
        }
        catch
        {
            return false;
        }
    }
}
