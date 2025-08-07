using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.MarketData.Contracts;
using Vertr.Platform.Common.Utils;
using Vertr.Platform.Host.Components.Common;

namespace Vertr.Platform.Host.Components.Pages;

public partial class History
{
    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    private IQueryable<CandlesHistoryItem> _candles { get; set; }

    private FluentDataGrid<CandlesHistoryItem> _dataGrid;

    private Instrument? _selectedInstrument;

    private Instrument[]? _instruments;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _instruments = await InitInstruments();
        _selectedInstrument = _instruments.Length > 0 ? _instruments[0] : null;
        _candles = await InitCandlesHistory();
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

    private async Task<IQueryable<CandlesHistoryItem>> InitCandlesHistory()
    {
        if (_selectedInstrument == null)
        {
            return Array.Empty<CandlesHistoryItem>().AsQueryable();
        }

        using var apiClient = _httpClientFactory.CreateClient("backend");
        var items = await apiClient.GetFromJsonAsync<CandlesHistoryItem[]>($"api/candles-history/{_selectedInstrument.Id}", JsonOptions.DefaultOptions);
        var res = items?.AsQueryable() ?? Array.Empty<CandlesHistoryItem>().AsQueryable();
        return res;
    }

    private async Task OnSelectedOptionChanged(Instrument? selected)
    {
        DemoLogger.WriteLine($"OnSelectedOptionChanged to {selected?.Name}");

        _selectedInstrument = selected;
        _candles = await InitCandlesHistory();
        await _dataGrid.RefreshDataAsync();
    }

}
