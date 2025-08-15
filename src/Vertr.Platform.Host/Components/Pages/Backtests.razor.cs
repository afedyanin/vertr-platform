using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.Backtest.Contracts;
using Vertr.Backtest.Contracts.Commands;
using Vertr.Platform.Common.Utils;
using Vertr.Platform.Host.Components.Common;
using Vertr.Platform.Host.Components.Models;
using Vertr.Strategies.Contracts;

namespace Vertr.Platform.Host.Components.Pages;

public partial class Backtests
{
    private PaginationState _pagination = new PaginationState() { ItemsPerPage = 12 };

    private FluentDataGrid<BacktestModel> dataGrid;

    private IQueryable<BacktestModel> _backtests { get; set; }

    private IDictionary<Guid, StrategyMetadata> _strategies { get; set; }

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _strategies = await InitStrategies();
        _backtests = await InitBacktests();
    }

    private async Task HandleCellClick(FluentDataGridCell<BacktestModel> cell)
    {
        if (cell.Item != null && cell.GridColumn <= 7)
        {
            await OpenPanelRightAsync(cell.Item);
        }
    }

    private async Task RefreshAsync()
    {
        _backtests = await InitBacktests();
        await dataGrid.RefreshDataAsync(force: true);
    }

    private async Task OpenDialogAsync()
    {
        var strategy = _strategies.Values.First();
        var initialModel = new BacktestModel()
        {
            Strategy = strategy,
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
                SubAccountId = Guid.NewGuid(),
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

        var saved = await SaveBacktest(model);

        if (saved)
        {
            ToastService.ShowSuccess($"Backtest {model.Backtest.Description} created.");
        }
        else
        {
            ToastService.ShowError($"Saving backtest {model.Backtest.Description} failed.");
        }

        await RefreshAsync();
    }


    private async Task OpenPanelRightAsync(BacktestModel backtestModel)
    {
        var dialog = await DialogService.ShowPanelAsync<BacktestPanel>(backtestModel, new DialogParameters<BacktestModel>()
        {
            Content = backtestModel,
            Alignment = HorizontalAlignment.Right,
            Title = $"{backtestModel.Backtest.Description}",
            PrimaryAction = "Close",
            SecondaryAction = null,
            Width = "400px",
        });

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

        if (model.DoCancel)
        {
            var cancelled = await CancelBacktest(model.Backtest.Id);

            if (cancelled)
            {
                await RefreshAsync();
                ToastService.ShowWarning($"Backtest {model.Backtest.Description} requested to cancel.");
            }
            else
            {
                ToastService.ShowError($"Backtest {model.Backtest.Description} failed to cancel.");
            }
        }

        if (model.DoStart)
        {
            var started = await StartBacktest(model.Backtest.Id);

            if (started)
            {
                await RefreshAsync();
                ToastService.ShowSuccess($"Backtest {model.Backtest.Description} enqueued to start.");
            }
            else
            {
                ToastService.ShowError($"Backtest {model.Backtest.Description} failed to start.");
            }
        }
    }

    private async Task<IQueryable<BacktestModel>> InitBacktests()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
        var backtests = await apiClient.GetFromJsonAsync<BacktestRun[]>("api/backtests", JsonOptions.DefaultOptions);

        if (backtests == null)
        {
            return Array.Empty<BacktestModel>().AsQueryable();
        }

        var modelItems = new List<BacktestModel>();

        foreach (var backtest in backtests)
        {
            if (_strategies.TryGetValue(backtest.StrategyId, out var strategy))
            {
                var item = new BacktestModel
                {
                    Backtest = backtest,
                    Strategy = strategy,
                };

                modelItems.Add(item);
            }
        }

        var res = modelItems?.AsQueryable() ?? Array.Empty<BacktestModel>().AsQueryable();
        return res;
    }

    private async Task<IDictionary<Guid, StrategyMetadata>> InitStrategies()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
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

    private async Task<bool> StartBacktest(Guid backtestId)
    {
        try
        {
            using var apiClient = _httpClientFactory.CreateClient("backend");
            var message = await apiClient.PutAsync($"api/backtests/start/{backtestId}", null);
            message.EnsureSuccessStatusCode();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> CancelBacktest(Guid backtestId)
    {
        try
        {
            using var apiClient = _httpClientFactory.CreateClient("backend");
            var message = await apiClient.PutAsync($"api/backtests/cancel/{backtestId}", null);
            message.EnsureSuccessStatusCode();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> SaveBacktest(BacktestModel backtestModel)
    {
        try
        {
            var request = new CreateBacktestRequest
            {
                Description = backtestModel.Backtest.Description,
                From = backtestModel.ComposeDateFrom(),
                To = backtestModel.ComposeDateTo(),
                StrategyId = backtestModel.Strategy.Id,
                SubAccountId = backtestModel.Backtest.SubAccountId,
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

    private async Task HandleDeleteAction(BacktestModel model)
    {
        var confirmation = await DialogService.ShowConfirmationAsync(
            $"Delete backtest: {model.Backtest.Description}?",
            "Yes",
            "No",
            $"Deleting backtest {model.Backtest.Description}");

        var result = await confirmation.Result;

        if (result.Cancelled)
        {
            return;
        }

        using var apiClient = _httpClientFactory.CreateClient("backend");
        var message = await apiClient.DeleteAsync($"api/backtests/{model.Backtest.Id}");
        message.EnsureSuccessStatusCode();

        await RefreshAsync();
        ToastService.ShowWarning($"Backtest {model.Backtest.Description} is deleted.");
    }
}
