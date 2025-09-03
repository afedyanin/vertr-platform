using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.MarketData.Contracts;
using Vertr.Platform.BlazorUI.Components.Common;
using Vertr.Platform.Common.Utils;

namespace Vertr.Platform.BlazorUI.Components.Pages;

public partial class Intraday
{
    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    private IQueryable<Candle> _candles { get; set; }

    private FluentDataGrid<Candle> _dataGrid;

    private Instrument? _selectedInstrument;

    private Instrument[]? _instruments;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _instruments = await InitInstruments();
        _selectedInstrument = _instruments.Length > 0 ? _instruments[0] : null;
        _candles = await InitCandles();
    }

    private async Task<Instrument[]> InitInstruments()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");

        var subscriptions = await apiClient.GetFromJsonAsync<CandleSubscription[]>("api/subscriptions", JsonOptions.DefaultOptions);
        var activeInstruments = subscriptions?.Where(x => x != null && !x.Disabled).Select(s => s.InstrumentId).ToHashSet();

        if (activeInstruments == null || activeInstruments.Count == 0)
        {
            return [];
        }

        var instruments = await apiClient.GetFromJsonAsync<Instrument[]>("api/instruments", JsonOptions.DefaultOptions);
        if (instruments == null)
        {
            return [];
        }

        var res = instruments.Where(x => activeInstruments.Contains(x.Id)).ToArray();
        return res ?? [];
    }

    private async Task<IQueryable<Candle>> InitCandles()
    {
        if (_selectedInstrument == null)
        {
            return Array.Empty<Candle>().AsQueryable();
        }

        using var apiClient = _httpClientFactory.CreateClient("backend");
        var items = await apiClient.GetFromJsonAsync<Candle[]>($"api/candles/{_selectedInstrument.Id}", JsonOptions.DefaultOptions);
        var res = items?.AsQueryable() ?? Array.Empty<Candle>().AsQueryable();
        return res;
    }

    private async Task OnSelectedOptionChanged(Instrument? selected)
    {
        DemoLogger.WriteLine($"OnSelectedOptionChanged to {selected?.Name}");

        _selectedInstrument = selected;
        _candles = await InitCandles();
        await _dataGrid.RefreshDataAsync();
    }
}
