using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Extensions;
using Vertr.Platform.BlazorUI.Components.Common;
using Vertr.Platform.BlazorUI.Components.Models;
using Vertr.Platform.Common.Utils;

namespace Vertr.Platform.BlazorUI.Components.Pages;

public partial class Subscriptions
{
    private IDialogReference? _dialog;

    private FluentDataGrid<SubscriptionModel> dataGrid;

    private IQueryable<SubscriptionModel> _subscriptions { get; set; }

    private IDictionary<Guid, Instrument> _instruments { get; set; }

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    [Inject]
    private ILogger<Subscriptions> _logger { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _instruments = await InitInstruments();
        _subscriptions = await InitSubscriptions();
    }

    private async Task HandleCellClick(FluentDataGridCell<SubscriptionModel> cell)
    {
        if (cell.Item != null && cell.GridColumn <= 6)
        {
            await OpenPanelRightAsync(cell.Item);
        }
    }
    private async Task AddSubscriptionAsync()
    {
        var instrument = _instruments.Values.FliterOutCurrency().First();

        _logger.LogDebug("Init instrument: {instument}", instrument.Name);

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
    }


    private async Task OpenPanelRightAsync(SubscriptionModel subscriptionModel)
    {
        _dialog = await DialogService.ShowPanelAsync<SubscriptionPanel>(subscriptionModel, new DialogParameters<SubscriptionModel>()
        {
            Content = subscriptionModel,
            Alignment = HorizontalAlignment.Right,
            Title = "Subscription",
            PrimaryAction = "Save",
            SecondaryAction = "Cancel",
            Width = "400px",
        });

        var result = await _dialog.Result;

        if (result.Cancelled)
        {
            // TODO: implement discard changes
            _subscriptions = await InitSubscriptions();
            await dataGrid.RefreshDataAsync(force: true);
            return;
        }

        if (result.Data is null)
        {
            return;
        }

        var model = result.Data as SubscriptionModel;

        if (model == null)
        {
            return;
        }

        model.Subscription.InstrumentId = model.Instrument.Id;
        _logger.LogDebug("Selected instrument from model: {instument}", model.Instrument.Name);

        var saved = await SaveSubscription(model.Subscription);
        if (saved)
        {
            ToastService.ShowSuccess($"Subscription for {model.Instrument.GetFullName()} ({model.Subscription.Interval}) saved.");
        }
        else
        {
            ToastService.ShowError($"Saving subscription {model.Instrument.GetFullName()} ({model.Subscription.Interval}) FAILED!");
        }

        _subscriptions = await InitSubscriptions();
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

    private async Task<IQueryable<SubscriptionModel>> InitSubscriptions()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
        var subscriptions = await apiClient.GetFromJsonAsync<CandleSubscription[]>("api/subscriptions", JsonOptions.DefaultOptions);

        if (subscriptions == null)
        {
            return Array.Empty<SubscriptionModel>().AsQueryable();
        }

        var modelItems = new List<SubscriptionModel>();

        foreach (var sub in subscriptions)
        {
            if (_instruments.TryGetValue(sub.InstrumentId, out var instrument))
            {
                var item = new SubscriptionModel
                {
                    Subscription = sub,
                    Instrument = instrument,
                };

                modelItems.Add(item);
            }
        }

        var res = modelItems?.AsQueryable() ?? Array.Empty<SubscriptionModel>().AsQueryable();
        return res;
    }

    private async Task<bool> SaveSubscription(CandleSubscription subscription)
    {
        try
        {
            using var apiClient = _httpClientFactory.CreateClient("backend");
            var content = JsonContent.Create(subscription);
            var message = await apiClient.PostAsync("api/subscriptions", content);
            message.EnsureSuccessStatusCode();
            return true;
        }
        catch
        {
            // TODO: Use toast service
            return false;
        }
    }

    private async Task HandleDeleteAction(SubscriptionModel model)
    {
        var confirmation = await DialogService.ShowConfirmationAsync(
            $"Delete subscription: {model.Instrument.GetFullName()}?",
            "Yes",
            "No",
            $"Deleting {model.Instrument.GetFullName()} Interval={model.Subscription.Interval}");

        var result = await confirmation.Result;

        if (result.Cancelled)
        {
            return;
        }

        using var apiClient = _httpClientFactory.CreateClient("backend");
        var message = await apiClient.DeleteAsync($"api/subscriptions/{model.Subscription.Id}");
        message.EnsureSuccessStatusCode();

        _subscriptions = await InitSubscriptions();
        await dataGrid.RefreshDataAsync(force: true);

        ToastService.ShowWarning($"Subscription for {model.Instrument.GetFullName()} ({model.Subscription.Interval}) deleted.");
    }
}
