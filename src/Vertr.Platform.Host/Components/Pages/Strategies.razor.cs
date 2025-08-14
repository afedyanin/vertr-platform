using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.MarketData.Contracts;
using Vertr.Platform.Common.Utils;
using Vertr.Platform.Host.Components.Common;
using Vertr.Platform.Host.Components.Models;
using Vertr.Strategies.Contracts;

namespace Vertr.Platform.Host.Components.Pages;

public partial class Strategies
{
    private IDialogReference? _dialog;

    private PaginationState _pagination = new PaginationState() { ItemsPerPage = 12 };

    private FluentDataGrid<StrategyModel> dataGrid;

    private IQueryable<StrategyModel> _strategies { get; set; }


    private IDictionary<Guid, Instrument> _instruments { get; set; }

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _instruments = await InitInstruments();
        _strategies = await InitStrategies();
    }

    private async Task HandleCellClick(FluentDataGridCell<StrategyModel> cell)
    {
        if (cell.Item != null && cell.GridColumn <= 6)
        {
            await OpenPanelRightAsync(cell.Item);
        }
    }

    private Task AddStrategyAsync()
    {
        DemoLogger.WriteLine("AddStrategyAsync");
        return Task.CompletedTask;

        /*
        var instrument = _instruments.Values.First();
        var model = new SubscriptionModel()
        {
            Instrument = instrument,
            Subscription = new CandleSubscription
            {
                Id = Guid.NewGuid(),
                InstrumentId = instrument.Id,
                Interval = CandleInterval.Min_1,
                Disabled = false,
                LoadHistory = true,
                ExternalStatus = null,
                ExternalSubscriptionId = null
            }
        };

        await OpenPanelRightAsync(model);
        */
    }

    private async Task OpenPanelRightAsync(StrategyModel strategyModel)
    {
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
            _strategies = await InitStrategies();
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
        var saved = await SaveStrategy(model.Strategy);
        if (saved)
        {
            DemoLogger.WriteLine($"Strategy {model.Strategy.Name} saved.");
        }
        else
        {
            DemoLogger.WriteLine($"Saving strategy {model.Strategy.Name} FAILED!");
        }

        _strategies = await InitStrategies();
        await dataGrid.RefreshDataAsync(force: true);
    }

    private async Task<IDictionary<Guid, Instrument>> InitInstruments()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
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

    private async Task<IQueryable<StrategyModel>> InitStrategies()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
        var strategies = await apiClient.GetFromJsonAsync<StrategyMetadata[]>("api/strategies", JsonOptions.DefaultOptions);

        if (strategies == null)
        {
            return Array.Empty<StrategyModel>().AsQueryable();
        }

        var modelItems = new List<StrategyModel>();

        foreach (var strategy in strategies)
        {
            if (_instruments.TryGetValue(strategy.InstrumentId, out var instrument))
            {
                var item = new StrategyModel
                {
                    Strategy = strategy,
                    Instrument = instrument,
                };

                modelItems.Add(item);
            }
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

        DemoLogger.WriteLine($"Strategy {model.Strategy.Name} is deleted.");
        return;

        /*
        using var apiClient = _httpClientFactory.CreateClient("backend");
        var message = await apiClient.DeleteAsync($"api/subscriptions/{model.Subscription.Id}");
        message.EnsureSuccessStatusCode();

        _subscriptions = await InitSubscriptions();
        await dataGrid.RefreshDataAsync(force: true);

        */
    }
}
