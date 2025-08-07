using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Vertr.Platform.Host.Components.Common;

public partial class CandlesHistoryGrid
{
    private IJSObjectReference? _jsModule;

    private ElementReference perspectiveViewer;

    [Parameter]
    public string TableName { get; set; } = "Table";

    [Parameter]
    public string Height { get; set; } = "800px";

    [Parameter]
    public Guid? HistoryItemId { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var schema = GetJsonSchema();
            var candles = await GetCandles();
            _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./Components/Common/CandlesGrid.razor.js");
            await _jsModule.InvokeVoidAsync("loadJson", schema, candles, perspectiveViewer);
        }
    }

    private async Task<Dictionary<string, object>[]> GetCandles()
    {
        if (!HistoryItemId.HasValue)
        {
            return [];
        }

        using var apiClient = _httpClientFactory.CreateClient("backend");
        var candles = await apiClient.GetFromJsonAsync<Dictionary<string, object>[]>($"api/candles-history/details/{HistoryItemId.Value}");

        return candles ?? [];
    }

    private Dictionary<string, string> GetJsonSchema()
        => new Dictionary<string, string>
        {
            {  "timeUtc", "datetime" },
            {  "open", "float" },
            {  "low", "float" },
            {  "high", "float" },
            {  "close", "float" },
            {  "volume", "float" },
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
}
