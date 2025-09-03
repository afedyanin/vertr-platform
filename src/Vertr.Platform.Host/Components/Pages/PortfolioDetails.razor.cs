using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.MarketData.Contracts;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.OrderExecution.WebApi.Requests;
using Vertr.Platform.Common.Utils;
using Vertr.Platform.Host.Components.Common;
using Vertr.Platform.Host.Components.Models;
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

    private bool _simulatedExecutionMode = false;

    private IQueryable<PositionModel> _positions { get; set; }

    private IDictionary<Guid, Instrument> _instruments { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");

        _simulatedExecutionMode = await InitExecutionMode(apiClient);
        _instruments = await InitInstruments(apiClient);
        await InitPortfolioData(apiClient);

        await base.OnParametersSetAsync();
    }

    private async Task InitPortfolioData(HttpClient apiClient)
    {
        _portfolio = await InitPortfolio(apiClient);
        _positions = InitPositions(_portfolio).AsQueryable();
    }

    private async Task<Portfolio?> InitPortfolio(HttpClient apiClient)
        => await apiClient.GetFromJsonAsync<Portfolio?>($"api/portfolios/{PortfolioId}", JsonOptions.DefaultOptions);

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

    private async Task<bool> InitExecutionMode(HttpClient apiClient)
        => await apiClient.GetFromJsonAsync<bool>("api/positions/simulated-order-execution", JsonOptions.DefaultOptions);

    private async Task HandleOpenPositionAction()
    {
        if (_portfolio == null)
        {
            return;
        }

        var openPositionModel = new OpenPositionModel
        {
            QuantityLots = 10,
            Price = 0,
            InstrumentId = GetDafultInstrumentId(),
            SelectedDate = DateTime.UtcNow,
            SelectedTime = DateTime.UtcNow,
            OrderExecutionSimulated = _simulatedExecutionMode
        };

        var dialog = await DialogService.ShowDialogAsync<OpenPositionDialog>(openPositionModel, new DialogParameters<OpenPositionModel>()
        {
            Content = openPositionModel,
            Alignment = HorizontalAlignment.Right,
            Title = "Open position",
            PrimaryAction = "Execute",
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

        if (model.InstrumentId == null)
        {
            return;
        }

        using var apiClient = _httpClientFactory.CreateClient("backend");

        try
        {
            var request = new OpenRequest(model.ComposeDate(), Guid.Parse(model.InstrumentId), _portfolio.Id, model.QuantityLots, model.Price);
            var content = JsonContent.Create(request, options: JsonOptions.DefaultOptions);
            var message = await apiClient.PostAsync("api/positions/open", content);
            message.EnsureSuccessStatusCode();
            var response = await message.Content.ReadFromJsonAsync<ExecuteOrderResponse>(options: JsonOptions.DefaultOptions);

            if (response == null)
            {
                throw new InvalidOperationException($"Invalid API response. StatusCode={message.StatusCode}");
            }

            if (!string.IsNullOrEmpty(response.OrderId))
            {
                ToastService.ShowSuccess($"New position opened. OrderId={response.OrderId}");
            }
            else if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                ToastService.ShowWarning($"New position is not opened! Message={response.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            DemoLogger.WriteLine($"Open position error: {ex.Message}");
            ToastService.ShowError($"Cannot open position. InstrumentId={model.InstrumentId} Qty=({model.QuantityLots})");
            return;
        }

        await RefreshPage(apiClient, 5000);
    }

    private async Task HandleClosePositionAction(PositionModel positionModel)
    {
        if (_portfolio == null)
        {
            return;
        }

        var closePositionModel = new CloseReversePostionModel
        {
            Price = 0,
            Instrument = positionModel.Instrument,
            Balance = positionModel.Position.Balance,
            SelectedDate = DateTime.UtcNow,
            SelectedTime = DateTime.UtcNow,
            OrderExecutionSimulated = _simulatedExecutionMode
        };

        var dialog = await DialogService.ShowDialogAsync<CloseReversePositionDialog>(closePositionModel, new DialogParameters<CloseReversePostionModel>()
        {
            Content = closePositionModel,
            Alignment = HorizontalAlignment.Right,
            Title = "Close position",
            PrimaryAction = "Execute",
            SecondaryAction = "Cancel",
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

        if (result.Data is not CloseReversePostionModel model)
        {
            return;
        }

        using var apiClient = _httpClientFactory.CreateClient("backend");

        try
        {
            var request = new CloseRequest(model.ComposeDate(), positionModel.Instrument.Id, _portfolio.Id, model.Price);
            var content = JsonContent.Create(request, options: JsonOptions.DefaultOptions);
            var message = await apiClient.PostAsync("api/positions/close", content);
            message.EnsureSuccessStatusCode();
            var response = await message.Content.ReadFromJsonAsync<ExecuteOrderResponse>(options: JsonOptions.DefaultOptions);

            if (response == null)
            {
                throw new InvalidOperationException($"Invalid API response. StatusCode={message.StatusCode}");
            }

            if (!string.IsNullOrEmpty(response.OrderId))
            {
                ToastService.ShowSuccess($"Position closed. OrderId={response.OrderId}");
            }
            else if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                ToastService.ShowWarning($"Position is not closed! Message={response.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            DemoLogger.WriteLine($"Close position error: {ex.Message}");
            ToastService.ShowError($"Cannot close position. Instrument={positionModel.Instrument.GetFullName()} Balance={positionModel.Position.Balance}");
            return;
        }

        await RefreshPage(apiClient, 5000);
    }

    private async Task HandleReversePositionAction(PositionModel positionModel)
    {
        if (_portfolio == null)
        {
            return;
        }

        var closePositionModel = new CloseReversePostionModel
        {
            Price = 0,
            Instrument = positionModel.Instrument,
            Balance = positionModel.Position.Balance,
            SelectedDate = DateTime.UtcNow,
            SelectedTime = DateTime.UtcNow,
            OrderExecutionSimulated = _simulatedExecutionMode
        };

        var dialog = await DialogService.ShowDialogAsync<CloseReversePositionDialog>(closePositionModel, new DialogParameters<CloseReversePostionModel>()
        {
            Content = closePositionModel,
            Alignment = HorizontalAlignment.Right,
            Title = "Reverse position",
            PrimaryAction = "Execute",
            SecondaryAction = "Cancel",
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

        if (result.Data is not CloseReversePostionModel model)
        {
            return;
        }

        using var apiClient = _httpClientFactory.CreateClient("backend");

        try
        {
            var request = new ReverseRequest(model.ComposeDate(), positionModel.Instrument.Id, _portfolio.Id, model.Price);
            var content = JsonContent.Create(request, options: JsonOptions.DefaultOptions);
            var message = await apiClient.PostAsync("api/positions/reverse", content);
            message.EnsureSuccessStatusCode();
            var response = await message.Content.ReadFromJsonAsync<ExecuteOrderResponse>(options: JsonOptions.DefaultOptions);

            if (response == null)
            {
                throw new InvalidOperationException($"Invalid API response. StatusCode={message.StatusCode}");
            }

            if (!string.IsNullOrEmpty(response.OrderId))
            {
                ToastService.ShowSuccess($"Position reverted. OrderId={response.OrderId}");
            }
            else if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                ToastService.ShowWarning($"Position is not reverted! Message={response.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            DemoLogger.WriteLine($"Reverse position error: {ex.Message}");
            ToastService.ShowError($"Cannot reverse position. Instrument={positionModel.Instrument.GetFullName()} Balance={positionModel.Position.Balance}");
            return;
        }

        await RefreshPage(apiClient, 5000);
    }

    private string GetDafultInstrumentId()
        => _instruments.Values.First(x =>
            !string.IsNullOrEmpty(x.InstrumentType) &&
            !x.InstrumentType.Equals("currency", StringComparison.OrdinalIgnoreCase)).Id.ToString();

    private async Task RefreshPage(HttpClient apiClient, int timeout = 1000)
    {
        await Task.Delay(timeout);
        await InitPortfolioData(apiClient);
        await dataGrid.RefreshDataAsync(force: true);
        await operationsGrid.RefreshDataAsync();
    }
}
