using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.MarketData.Contracts;
using Vertr.Platform.BlazorUI.Components.Models;
using Vertr.Platform.Common.Utils;

namespace Vertr.Platform.BlazorUI.Components.Pages;

public partial class StockTicker
{
    private FluentDataGrid<StockModel> dataGrid;

    private IQueryable<StockModel> _stocks { get; set; }

    private IDictionary<Guid, Instrument> _instruments { get; set; }

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");

        _instruments = await InitInstruments(apiClient);
        _stocks = await InitStocks();

        await base.OnInitializedAsync();
    }
    private async Task<IDictionary<Guid, Instrument>> InitInstruments(HttpClient apiClient)
    {
        var instruments = await apiClient.GetFromJsonAsync<Instrument[]>("api/instruments", JsonOptions.DefaultOptions);
        var res = new Dictionary<Guid, Instrument>();

        if (instruments == null)
        {
            return res;
        }

        var filterd = instruments?
            .Where(x =>
                !string.IsNullOrEmpty(x.InstrumentType) &&
                !x.InstrumentType.Equals("currency", StringComparison.OrdinalIgnoreCase))
            .ToArray() ?? [];


        foreach (var instrument in filterd)
        {
            res[instrument.Id] = instrument;
        }

        return res;
    }

    private Task<IQueryable<StockModel>> InitStocks()
    {
        var res = new List<StockModel>();

        res.Add(new StockModel
        {
            Symbol = "AAA",
            DayOpen = 100,
            DayLow = 90,
            DayHigh = 120,
            LastChange = 103,
            Change = 3,
            PercentChange = 0.04,
            UpdatedAt = DateTime.UtcNow,
        });

        res.Add(new StockModel
        {
            Symbol = "BBB",
            DayOpen = 100,
            DayLow = 90,
            DayHigh = 120,
            LastChange = 103,
            Change = 3,
            PercentChange = 0.04,
            UpdatedAt = DateTime.UtcNow,
        });

        res.Add(new StockModel
        {
            Symbol = "CCC",
            DayOpen = 100,
            DayLow = 90,
            DayHigh = 120,
            LastChange = 103,
            Change = 3,
            PercentChange = 0.04,
            UpdatedAt = DateTime.UtcNow,
        });

        return Task.FromResult(res.AsQueryable());
    }
}
