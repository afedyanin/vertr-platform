using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.Backtest.Contracts;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Extensions;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.Platform.BlazorUI.Components.Common;
using Vertr.Platform.BlazorUI.Components.Models;
using Vertr.Platform.Common.Utils;
using Vertr.PortfolioManager.Contracts;
using Vertr.Strategies.Contracts;

namespace Vertr.Platform.BlazorUI.Components.Pages;

public partial class PortfolioDetails : IAsyncDisposable
{
    private Portfolio? _portfolio;

    private HubConnection _hubConnection;

    private bool _isConnected =>
           _hubConnection?.State == HubConnectionState.Connected;

    [Parameter]
    public string PortfolioId { get; set; }

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    private FluentDataGrid<PositionModel> dataGrid;

    private TradeOperationsGrid operationsGrid;

    private bool _simulatedExecutionMode;

    private Guid? _backtestId;

    private Guid? _strategyId;

    private Dictionary<Guid, PositionModel> _positionsDict;

    private IQueryable<PositionModel> _positions => _positionsDict.Values.OrderBy(p => p.Position.InstrumentId).AsQueryable();

    private IDictionary<Guid, Instrument> _instruments { get; set; }

    protected override async Task OnInitializedAsync()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");

        _simulatedExecutionMode = await InitExecutionMode(apiClient);

        var backtest = await InitBacktest(apiClient);
        _backtestId = backtest == null ? Guid.Empty : backtest.Id;

        var strategyMetadata = await InitStrategy(apiClient);
        _strategyId = strategyMetadata == null ? Guid.Empty : strategyMetadata.Id;

        _instruments = await InitInstruments(apiClient);
        _portfolio = await InitPortfolio(apiClient);
        _positionsDict = InitPositions(_portfolio);

        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender && _portfolio != null && !_portfolio.IsBacktest)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(Navigation.ToAbsoluteUri("/positionsHub"))
                .Build();

            await _hubConnection.StartAsync();
            await StartStreaming();
        }
    }

    private async Task StartStreaming()
    {
        var stream = _hubConnection.StreamAsync<Position>("StreamPositions");

        await foreach (var position in stream)
        {
            if (position.PortfolioId.ToString() != PortfolioId)
            {
                continue;
            }

            var model = CreateModel(position);
            if (model != null)
            {
                _positionsDict[model.Position.Id] = model;
                StateHasChanged();
                await dataGrid.RefreshDataAsync();
                await operationsGrid.RefreshDataAsync();
            }
        }
    }

    private async Task<Portfolio?> InitPortfolio(HttpClient apiClient)
        => await apiClient.GetFromJsonAsync<Portfolio?>($"api/portfolios/{PortfolioId}", JsonOptions.DefaultOptions);

    private Dictionary<Guid, PositionModel> InitPositions(Portfolio? portfolio)
    {
        var res = new Dictionary<Guid, PositionModel>();

        if (portfolio == null)
        {
            return res;
        }

        foreach (var position in portfolio.Positions)
        {
            var model = CreateModel(position);
            if (model != null)
            {
                res[model.Position.Id] = model;
            }
        }

        return res;
    }

    private PositionModel? CreateModel(Position position)
    {
        _instruments.TryGetValue(position.InstrumentId, out var instrument);

        if (instrument == null)
        {
            return null;
        }

        var model = new PositionModel
        {
            Position = position,
            Instrument = instrument,
        };

        return model;
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

    private async Task<BacktestRun?> InitBacktest(HttpClient apiClient)
    {
        try
        {
            var res = await apiClient.GetFromJsonAsync<BacktestRun?>($"api/backtests/by-portfolio/{PortfolioId}", JsonOptions.DefaultOptions);
            return res;
        }
        catch
        {
            return null;
        }
    }

    private async Task<StrategyMetadata?> InitStrategy(HttpClient apiClient)
    {
        try
        {
            var res = await apiClient.GetFromJsonAsync<StrategyMetadata?>($"api/strategies/by-portfolio/{PortfolioId}", JsonOptions.DefaultOptions);
            return res;
        }
        catch
        {
            return null;
        }
    }

    private async Task HandleRefresh()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
        _portfolio = await InitPortfolio(apiClient);
        _positionsDict = InitPositions(_portfolio);

        await dataGrid.RefreshDataAsync();
        await operationsGrid.RefreshDataAsync();
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
    }

    private string GetDafultInstrumentId()
        => _instruments.Values.FliterOutCurrency().First().Id.ToString();

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
