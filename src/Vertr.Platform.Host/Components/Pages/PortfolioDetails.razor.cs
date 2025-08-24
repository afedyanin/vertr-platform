using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.MarketData.Contracts;
using Vertr.Platform.Common.Utils;
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

    private IQueryable<PositionModel> _positions { get; set; }

    private IDictionary<Guid, Instrument> _instruments { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        _instruments = await InitInstruments();
        _portfolio = await InitPortfolio();
        _positions = InitPositions(_portfolio).AsQueryable();

        await base.OnParametersSetAsync();
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

}
