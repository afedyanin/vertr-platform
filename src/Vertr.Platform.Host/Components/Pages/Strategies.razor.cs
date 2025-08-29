using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.MarketData.Contracts;
using Vertr.Platform.Common.Utils;
using Vertr.Platform.Host.Components.Common;
using Vertr.Platform.Host.Components.Models;
using Vertr.PortfolioManager.Contracts;
using Vertr.Strategies.Contracts;

namespace Vertr.Platform.Host.Components.Pages;

public partial class Strategies
{
    private IDialogReference? _dialog;

    private PaginationState _pagination = new PaginationState() { ItemsPerPage = 12 };

    private FluentDataGrid<StrategyModel> dataGrid;

    private IQueryable<StrategyModel> _strategies { get; set; }


    private IDictionary<Guid, Instrument> _instruments { get; set; }

    private IDictionary<Guid, Portfolio> _portfolios { get; set; }

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");

        _instruments = await InitInstruments(apiClient);
        _portfolios = await InitPortfolios(apiClient);
        _strategies = await InitStrategies(apiClient);

        await base.OnInitializedAsync();
    }

    private async Task HandleCellClick(FluentDataGridCell<StrategyModel> cell)
    {
        if (cell.Item != null && cell.GridColumn <= 6)
        {
            await OpenPanelRightAsync(cell.Item);
        }
    }

    private async Task AddStrategyAsync()
    {
        var instrument = _instruments.Values.First();
        var portfolio = _portfolios.Values.First();

        var model = new StrategyModel()
        {
            Instrument = instrument,
            Portfolio = portfolio,
            Strategy = new StrategyMetadata
            {
                Id = Guid.NewGuid(),
                PortfolioId = portfolio.Id,
                CreatedAt = DateTime.UtcNow,
                InstrumentId = instrument.Id,
                Type = StrategyType.RandomWalk,
                Name = "Test Strategy",
                IsActive = false,
                QtyLots = 10
            }
        };

        await OpenPanelRightAsync(model);
    }

    private async Task OpenPanelRightAsync(StrategyModel strategyModel)
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");

        _dialog = await DialogService.ShowPanelAsync<StrategyPanel>(strategyModel, new DialogParameters<StrategyModel>()
        {
            Content = strategyModel,
            Alignment = HorizontalAlignment.Right,
            Title = $"{strategyModel.Strategy.Name}",
            PrimaryAction = "Save",
            SecondaryAction = "Cancel",
            Width = "400px",
        });

        var result = await _dialog.Result;

        if (result.Cancelled)
        {
            // TODO: implement discard changes
            _strategies = await InitStrategies(apiClient);
            await dataGrid.RefreshDataAsync(force: true);
            return;
        }

        if (result.Data is null)
        {
            return;
        }


        if (result.Data is not StrategyModel model)
        {
            return;
        }

        model.Strategy.InstrumentId = model.Instrument.Id;
        model.Strategy.PortfolioId = model.Portfolio.Id;

        var saved = await SaveStrategy(model.Strategy);
        if (saved)
        {
            DemoLogger.WriteLine($"Strategy {model.Strategy.Name} saved.");
        }
        else
        {
            DemoLogger.WriteLine($"Saving strategy {model.Strategy.Name} FAILED!");
        }

        _strategies = await InitStrategies(apiClient);
        await dataGrid.RefreshDataAsync(force: true);
    }

    private async Task<IDictionary<Guid, Instrument>> InitInstruments(HttpClient apiClient)
    {
        var instruments = await apiClient.GetFromJsonAsync<Instrument[]>("api/instruments", JsonOptions.DefaultOptions);
        var res = new Dictionary<Guid, Instrument>();

        if (instruments == null)
        {
            return res;
        }

        foreach (var instrument in instruments)
        {
            res[instrument.Id] = instrument;
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

    private async Task<IQueryable<StrategyModel>> InitStrategies(HttpClient apiClient)
    {
        var strategies = await apiClient.GetFromJsonAsync<StrategyMetadata[]>("api/strategies", JsonOptions.DefaultOptions);

        if (strategies == null)
        {
            return Array.Empty<StrategyModel>().AsQueryable();
        }

        var modelItems = new List<StrategyModel>();

        foreach (var strategy in strategies)
        {
            if (!_instruments.TryGetValue(strategy.InstrumentId, out var instrument))
            {
                // Instrument is not found
                continue;
            }

            if (!_portfolios.TryGetValue(strategy.PortfolioId, out var portfolio))
            {
                // Portfolio is not found
                continue;
            }

            var item = new StrategyModel
            {
                Strategy = strategy,
                Instrument = instrument,
                Portfolio = portfolio
            };

            modelItems.Add(item);
        }

        var res = modelItems?.AsQueryable() ?? Array.Empty<StrategyModel>().AsQueryable();
        return res;
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
        catch
        {
            // TODO: Use toast service
            return false;
        }
    }

    private async Task HandleDeleteAction(StrategyModel model)
    {
        var confirmation = await DialogService.ShowConfirmationAsync(
            $"Delete strategy: {model.Strategy.Name}?",
            "Yes",
            "No",
            $"Deleting {model.Strategy.Name}");

        var result = await confirmation.Result;

        if (result.Cancelled)
        {
            return;
        }

        using var apiClient = _httpClientFactory.CreateClient("backend");
        var message = await apiClient.DeleteAsync($"api/strategies/{model.Strategy.Id}");
        message.EnsureSuccessStatusCode();

        _strategies = await InitStrategies(apiClient);
        await dataGrid.RefreshDataAsync(force: true);

        DemoLogger.WriteLine($"Strategy {model.Strategy.Name} is deleted.");
        return;
    }
}
