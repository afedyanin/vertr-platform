using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.Backtest.Contracts;
using Vertr.Platform.Common.Utils;
using Vertr.Platform.Host.Components.Common;
using Vertr.Platform.Host.Components.Models;
using Vertr.Strategies.Contracts;

namespace Vertr.Platform.Host.Components.Pages;

public partial class Backtests
{
    private IDialogReference? _dialog;

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
        if (cell.Item != null && cell.GridColumn <= 6)
        {
            await OpenPanelRightAsync(cell.Item);
        }
    }

    private async Task AddBacktestAsync()
    {
        var strategy = _strategies.Values.First();
        var model = new BacktestModel()
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

        await OpenPanelRightAsync(model);
    }

    private async Task OpenPanelRightAsync(BacktestModel backtestModel)
    {
        _dialog = await DialogService.ShowPanelAsync<BacktestPanel>(backtestModel, new DialogParameters<BacktestModel>()
        {
            Content = backtestModel,
            Alignment = HorizontalAlignment.Right,
            Title = $"{backtestModel.Backtest.Description}",
            PrimaryAction = "Save",
            SecondaryAction = "Cancel",
            Width = "400px",
        });

        var result = await _dialog.Result;

        if (result.Cancelled)
        {
            _backtests = await InitBacktests();
            await dataGrid.RefreshDataAsync(force: true);
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

        model.Backtest.StrategyId = model.Strategy.Id;
        var saved = await SaveBacktest(model.Backtest);

        if (saved)
        {
            DemoLogger.WriteLine($"Backtest {model.Backtest.Description} saved.");
        }
        else
        {
            DemoLogger.WriteLine($"Saving backtest {model.Backtest.Description} FAILED!");
        }

        _backtests = await InitBacktests();
        await dataGrid.RefreshDataAsync(force: true);
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

    private async Task<bool> SaveBacktest(BacktestRun backtest)
    {
        try
        {
            using var apiClient = _httpClientFactory.CreateClient("backend");
            var content = JsonContent.Create(backtest);
            var message = await apiClient.PostAsync("api/backtests", content);
            message.EnsureSuccessStatusCode();
            return true;
        }
        catch
        {
            // TODO: Use toast service
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

        _backtests = await InitBacktests();
        await dataGrid.RefreshDataAsync(force: true);

        DemoLogger.WriteLine($"Backtest {model.Backtest.Description} is deleted.");
        return;
    }
}
