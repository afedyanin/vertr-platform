using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Vertr.MarketData.Contracts;
using Vertr.Platform.BlazorUI.Components.Models;
using Vertr.Platform.Common.Utils;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Extensions;

namespace Vertr.Platform.BlazorUI.Components.Common;

public partial class TradeOperationsGrid
{
    private IJSObjectReference? _jsModule;

    private ElementReference perspectiveViewer;

    [Parameter]
    public string TableName { get; set; } = "Table";

    [Parameter]
    public string Height { get; set; } = "800px";

    [Parameter]
    public string? PortfolioId { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    private IDictionary<Guid, Instrument> _instruments { get; set; }

    public async Task RefreshDataAsync()
    {
        var schema = GetJsonSchema();
        var operations = await GetOperations();
        var operationsJson = JsonSerializer.Serialize(operations, JsonOptions.DefaultOptions);

        _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Vertr.Platform.BlazorUI.Components/Common/TradeOperationsGrid.razor.js");
        await _jsModule.InvokeVoidAsync("loadJson", schema, operationsJson, perspectiveViewer);
    }

    protected override async Task OnParametersSetAsync()
    {
        _instruments = await InitInstruments();
        await base.OnParametersSetAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await RefreshDataAsync();
        }
    }

    private async Task<TradeOperationModel[]> GetOperations()
    {
        if (string.IsNullOrEmpty(PortfolioId))
        {
            return [];
        }

        using var apiClient = _httpClientFactory.CreateClient("backend");
        var operations = await apiClient.GetFromJsonAsync<TradeOperation[]>($"api/trade-operations/{PortfolioId}", JsonOptions.DefaultOptions);

        if (operations == null)
        {
            return [];
        }

        var items = new List<TradeOperationModel>();

        foreach (var operation in operations)
        {
            var instrumentName = "Unknown";

            if (_instruments.TryGetValue(operation.InstrumentId, out var instrument))
            {
                instrumentName = instrument.GetFullName();
            }

            var isIncomeOperationType = operation.OperationType.IsIncome();

            var isIncome = !isIncomeOperationType.HasValue && operation.Amount.Value > 0
                || isIncomeOperationType.HasValue && isIncomeOperationType.Value;

            var incomeAmount = isIncome ? operation.Amount.Value : decimal.Zero;
            var outcomeAmount = isIncome ? decimal.Zero : operation.Amount.Value;

            var model = new TradeOperationModel
            {
                CreatedAt = operation.CreatedAt,
                OperationType = operation.OperationType.GetFullName(),
                Instrument = instrumentName,
                IncomeAmount = incomeAmount,
                OutcomeAmount = outcomeAmount,
                Currency = operation.Amount.Currency,
                Price = operation.Price.Value,
                Quantity = operation.Quantity,
                OrderId = operation.OrderId,
            };

            items.Add(model);
        }

        return [.. items];
    }

    private Dictionary<string, string> GetJsonSchema()
        => new Dictionary<string, string>
        {
            {  "createdAt", "datetime" },
            {  "operationType", "string" },
            {  "instrument", "string" },
            {  "incomeAmount", "float" },
            {  "outcomeAmount", "float" },
            {  "currency", "string" },
            {  "price", "float" },
            {  "quantity", "float" },
            {  "orderId", "string" },
        };

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_jsModule != null)
            {
                // await _jsModule.InvokeVoidAsync("dispose");
                await _jsModule.DisposeAsync();
            }
        }
        catch (JSDisconnectedException)
        {
            // Client disconnected.
        }
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
}

