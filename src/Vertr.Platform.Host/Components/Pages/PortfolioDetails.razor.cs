using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.MarketData.Contracts;
using Vertr.Platform.Common.Utils;
using Vertr.Platform.Host.Components.Common;
using Vertr.Platform.Host.Components.Models;
using Vertr.Platform.Host.Requests;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.Platform.Host.Components.Pages;

public partial class PortfolioDetails
{
    private Portfolio? _portfolio;

    [Parameter]
    public string PortfolioId { get; set; }

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    private FluentDataGrid<PositionModel> dataGrid;

    private TradeOperationsGrid operationsGrid;

    private IQueryable<PositionModel> _positions { get; set; }

    private IDictionary<Guid, Instrument> _instruments { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        _instruments = await InitInstruments();
        await InitPortfolioData();

        await base.OnParametersSetAsync();
    }

    private async Task InitPortfolioData()
    {
        _portfolio = await InitPortfolio();
        _positions = InitPositions(_portfolio).AsQueryable();
    }

    private async Task<Portfolio?> InitPortfolio()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
        var portfolio = await apiClient.GetFromJsonAsync<Portfolio?>($"api/portfolios/{PortfolioId}", JsonOptions.DefaultOptions);
        return portfolio;
    }

    private List<PositionModel> InitPositions(Portfolio? portfolio)
    {
        var res = new List<PositionModel>();

        if (portfolio == null)
        {
            return res;
        }

        foreach (var position in portfolio.Positions)
        {
            _instruments.TryGetValue(position.InstrumentId, out var instrument);

            if (instrument == null)
            {
                // TODO: Handle unknown instrument
                continue;
            }

            var model = new PositionModel
            {
                Position = position,
                Instrument = instrument,
            };

            res.Add(model);
        }

        return res;
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

    private async Task HandleDeleteAction()
    {
        if (_portfolio == null)
        {
            return;
        }

        var confirmation = await DialogService.ShowConfirmationAsync(
            $"Delete portfolio: {_portfolio.Name}?",
            "Yes",
            "No",
            $"Deleting portfolio {_portfolio.Name}");

        var result = await confirmation.Result;

        if (result.Cancelled)
        {
            return;
        }

        using var apiClient = _httpClientFactory.CreateClient("backend");
        var message = await apiClient.DeleteAsync($"api/portfolios/{_portfolio.Id}");
        message.EnsureSuccessStatusCode();

        ToastService.ShowWarning($"Portfolio Id={_portfolio.Id} deleted.");

        Navigation.NavigateTo("/portfolios");
    }

    private async Task HandleDepositAction()
    {
        if (_portfolio == null)
        {
            return;
        }

        var depositModel = new DepositModel
        {
            Amount = 100_000,
            Currency = "rub",
            SelectedDate = DateTime.UtcNow,
            SelectedTime = DateTime.UtcNow,
        };

        var dialog = await DialogService.ShowDialogAsync<DepositDialog>(depositModel, new DialogParameters<DepositModel>()
        {
            Content = depositModel,
            Alignment = HorizontalAlignment.Right,
            Title = "Make deposit",
            PrimaryAction = "Save",
            SecondaryAction = "Cancel",
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

        if (result.Data is not DepositModel model)
        {
            return;
        }

        try
        {
            using var apiClient = _httpClientFactory.CreateClient("backend");
            var request = new DepositRequest(model.ComposeDate(), _portfolio.Id, model.Amount, model.Currency);
            var content = JsonContent.Create(request);
            var message = await apiClient.PutAsync("api/portfolios/deposit", content);
            message.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            DemoLogger.WriteLine($"Making deposit error: {ex.Message}");
            ToastService.ShowError($"Cannot save deposit amount: {model.Amount} ({model.Currency})");
            return;
        }

        ToastService.ShowSuccess($"Added deposit amount: {model.Amount} ({model.Currency})");

        await RefreshPage();
    }

    private async Task HandleOpenPositionAction()
    {
        if (_portfolio == null)
        {
            return;
        }

        var openPositionModel = new OpenPositionModel
        {
            QuantityLots = 10,
            Price = 100,
            Instrument = _instruments.Values.First(x =>
                !string.IsNullOrEmpty(x.InstrumentType) &&
                !x.InstrumentType.Equals("currency", StringComparison.OrdinalIgnoreCase)),
            SelectedDate = DateTime.UtcNow,
            SelectedTime = DateTime.UtcNow,
        };

        var dialog = await DialogService.ShowDialogAsync<OpenPositionDialog>(openPositionModel, new DialogParameters<OpenPositionModel>()
        {
            Content = openPositionModel,
            Alignment = HorizontalAlignment.Right,
            Title = "Open position",
            PrimaryAction = "Save",
            SecondaryAction = "Cancel",
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

        if (result.Data is not OpenPositionModel model)
        {
            return;
        }

        try
        {
            using var apiClient = _httpClientFactory.CreateClient("backend");
            var request = new OpenRequest(model.ComposeDate(), model.Instrument.Id, _portfolio.Id, model.QuantityLots, model.Price);
            var content = JsonContent.Create(request);
            var message = await apiClient.PostAsync("api/positions/open", content);
            message.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            DemoLogger.WriteLine($"Open position error: {ex.Message}");
            ToastService.ShowError($"Cannot open position. InstrumentId={model.Instrument} Qty=({model.QuantityLots})");
            return;
        }

        ToastService.ShowSuccess($"New position opened!");

        await RefreshPage(5000);
    }

    private async Task RefreshPage(int timeout = 1000)
    {
        await Task.Delay(timeout);
        await InitPortfolioData();
        await dataGrid.RefreshDataAsync(force: true);
        await operationsGrid.RefreshDataAsync();
    }
}
