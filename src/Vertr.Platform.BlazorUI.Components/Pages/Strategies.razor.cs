using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Extensions;
using Vertr.Platform.BlazorUI.Components.Common;
using Vertr.Platform.BlazorUI.Components.Models;
using Vertr.Platform.Common.Utils;
using Vertr.PortfolioManager.Contracts;
using Vertr.Strategies.Contracts;

namespace Vertr.Platform.BlazorUI.Components.Pages;

public partial class Strategies
{
    private IDialogReference? _dialog;

    private FluentDataGrid<StrategyModel> dataGrid;

    private IQueryable<StrategyModel> _strategies { get; set; }

    private IDictionary<Guid, Instrument> _instruments { get; set; }

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");

        _instruments = await InitInstruments(apiClient);
        _strategies = await InitStrategies(apiClient);

        await base.OnInitializedAsync();
    }

    private void HandleCellClick(FluentDataGridCell<StrategyModel> cell)
    {
        if (cell.Item == null)
        {
            return;
        }

        Navigation.NavigateTo($"strategies/details/{cell.Item.Strategy.Id}");
    }

    private async Task AddStrategyAsync()
    {
        var instrument = _instruments.Values.First();

        var model = new StrategyModel()
        {
            Instrument = instrument,
            Strategy = new StrategyMetadata
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                InstrumentId = instrument.Id,
                Type = StrategyType.RandomWalk,
                Name = "Strategy",
                IsActive = false,
                QtyLots = 10
            }
        };

        _dialog = await DialogService.ShowDialogAsync<StrategyDialog>(model, new DialogParameters<StrategyModel>()
        {
            Content = model,
            Alignment = HorizontalAlignment.Center,
            Title = "Create new strategy",
            PrimaryAction = "Save",
            SecondaryAction = "Cancel",
            Width = "500px",
            Height = "580px",
            TrapFocus = true,
            Modal = true,
            PreventScroll = true
        });

        var result = await _dialog.Result;

        using var apiClient = _httpClientFactory.CreateClient("backend");

        if (result.Cancelled)
        {
            // TODO: implement discard changes
            _strategies = await InitStrategies(apiClient);
            StateHasChanged();
            return;
        }

        if (result.Data is null)
        {
            return;
        }

        if (result.Data is not StrategyModel strategyModel)
        {
            return;
        }

        var portfolio = new Portfolio
        {
            Id = Guid.NewGuid(),
            IsBacktest = false,
            Name = $"Portfolio for {strategyModel.Strategy.Name} strategy",
            UpdatedAt = model.Strategy.CreatedAt,
        };

        var savedPortfolio = await SavePortfolio(portfolio);

        if (!savedPortfolio)
        {
            DemoLogger.WriteLine($"Error creating portfolio for strategy {model.Strategy.Name}");
            return;
        }

        strategyModel.Strategy.PortfolioId = portfolio.Id;
        strategyModel.Strategy.InstrumentId = strategyModel.Instrument.Id;

        var saved = await SaveStrategy(strategyModel.Strategy);

        if (!saved)
        {
            ToastService.ShowError($"Saving strategy {model.Strategy.Name} failed.");
            return;

        }

        Navigation.NavigateTo($"strategies/details/{strategyModel.Strategy.Id}");
    }

    private async Task<IDictionary<Guid, Instrument>> InitInstruments(HttpClient apiClient)
    {
        var instruments = await apiClient.GetFromJsonAsync<Instrument[]>("api/instruments", JsonOptions.DefaultOptions);
        var res = new Dictionary<Guid, Instrument>();

        if (instruments == null)
        {
            return res;
        }

        var filterd = instruments.FliterOutCurrency();

        foreach (var instrument in filterd)
        {
            res[instrument.Id] = instrument;
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

            var item = new StrategyModel
            {
                Strategy = strategy,
                Instrument = instrument,
            };

            modelItems.Add(item);
        }

        var res = modelItems?.AsQueryable() ?? Array.Empty<StrategyModel>().AsQueryable();
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
}
