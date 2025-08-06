using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.MarketData.Contracts;

namespace Vertr.Platform.Host.Components.Pages;

public partial class Intraday
{
    private IQueryable<Candle> _candles { get; set; }

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    private FluentDataGrid<Candle> dataGrid;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _candles = await InitCandles();
    }

    private async Task<IQueryable<Candle>> InitCandles()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
        var items = await apiClient.GetFromJsonAsync<Candle[]>("api/candles/e6123145-9665-43e0-8413-cd61b8aa9b13"); // SBER
        var res = items?.AsQueryable() ?? Array.Empty<Candle>().AsQueryable();
        return res;
    }
}
