using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.MarketData.Contracts;
using Vertr.Platform.Common.Utils;
using Vertr.Platform.Host.Components.Common;
using Vertr.Platform.Host.Components.Models;

namespace Vertr.Platform.Host.Components.Pages;

public partial class Subscriptions
{
    private IDialogReference? _dialog;

    private PaginationState _pagination = new PaginationState() { ItemsPerPage = 12 };

    private FluentDataGrid<SubscriptionModel> dataGrid;

    private IQueryable<SubscriptionModel> _subscriptions { get; set; }

    private IDictionary<Guid, Instrument> _instruments { get; set; }

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

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

    private async Task OpenPanelRightAsync(SubscriptionModel subscriptionModel)
    {/*
        _dialog = await DialogService.ShowPanelAsync<InstrumentPanel>(instrument, new DialogParameters<Instrument>()
        {
            Content = instrument,
            Alignment = HorizontalAlignment.Right,
            Title = $"{instrument.Name}",
            PrimaryAction = "Close",
            SecondaryAction = null,
            Width = "400px",
        });
        DialogResult result = await _dialog.Result;
        */
    }

    private async Task OpenDialogAsync()
    {
        var dummyInstrument = new Instrument
        {
            Symbol = new Symbol("", "")
        };

        var parameters = new DialogParameters()
        {
            Title = "Search instrument",
            PrimaryAction = "Select",
            SecondaryAction = null,
            Width = "500px",
            TrapFocus = true,
            Modal = true,
            PreventScroll = true
        };

        var dialog = await DialogService.ShowDialogAsync<InstrumentScreener>(dummyInstrument, parameters);
        var result = await dialog.Result;

        if (result.Data is null)
        {
            return;
        }

        var selectedInstrument = result.Data as Instrument;
        var instrumentId = selectedInstrument?.Id;

        if (instrumentId == null)
        {
            return;
        }

        //await AddInstrument(instrumentId.Value);
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

        foreach(var sub in subscriptions)
        {
            if (_instruments.TryGetValue(sub.InstrumentId, out var instrument))
            {
                var item = new SubscriptionModel
                {
                    Subscription = sub,
                    InstrumentName = $"{instrument.Symbol.ClassCode}.{instrument.Symbol.Ticker} ({instrument.Name})",
                };

                modelItems.Add(item);
            }
        }

        var res = modelItems?.AsQueryable() ?? Array.Empty<SubscriptionModel>().AsQueryable();
        return res;
    }

    /*
    private async Task AddSubscription(Guid instrumentId)
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
        var instrument = await apiClient.GetFromJsonAsync<Instrument>($"api/tinvest/instrument-by-id/{instrumentId}");

        if (instrument == null)
        {
            return;
        }

        var content = JsonContent.Create(instrument);
        await apiClient.PostAsync("api/instruments", content);
        _instrumentList = await InitInstruments();

        DemoLogger.WriteLine($"{instrument.Symbol.ClassCode}.{instrument.Symbol.Ticker} ({instrument.Name}) added.");
    }
    */
    private async Task HandleDeleteAction(SubscriptionModel model)
    {
        var confirmation = await DialogService.ShowConfirmationAsync(
            $"Delete subscription: {model.InstrumentName}?",
            "Yes",
            "No",
            $"Deleting {model.InstrumentName} Interval={model.Subscription.Interval}");

        var result = await confirmation.Result;

        if (result.Cancelled)
        {
            return;
        }

        /*
        using var apiClient = _httpClientFactory.CreateClient("backend");
        await apiClient.DeleteAsync($"api/instruments/{instrument.Id}");
        _instrumentList = await InitInstruments();
        await dataGrid.RefreshDataAsync(force: true);

        DemoLogger.WriteLine($"{instrument.Symbol.ClassCode}.{instrument.Symbol.Ticker} ({instrument.Name}) deleted.");
        */
    }
}
