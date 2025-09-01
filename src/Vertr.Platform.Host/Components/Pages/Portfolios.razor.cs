using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.Platform.Common.Utils;
using Vertr.Platform.Host.Components.Common;
using Vertr.Platform.Host.Components.Models;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.Platform.Host.Components.Pages;

public partial class Portfolios
{
    private IDialogReference? _dialog;

    private PaginationState _pagination = new PaginationState() { ItemsPerPage = 12 };

    private FluentDataGrid<PortfolioModel> dataGrid;

    private IQueryable<PortfolioModel> _portfolios { get; set; }

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _portfolios = await InitPortfolios();
    }

    private Task HandleCellClick(FluentDataGridCell<PortfolioModel> cell)
    {
        if (cell.Item is not null)
        {
            NavigationManager.NavigateTo($"/portfolios/details/{cell.Item.Portfolio.Id}");
        }

        return Task.CompletedTask;
    }

    private async Task<IQueryable<PortfolioModel>> InitPortfolios()
    {
        using var apiClient = _httpClientFactory.CreateClient("backend");
        var portfolios = await apiClient.GetFromJsonAsync<Portfolio[]>("api/portfolios", JsonOptions.DefaultOptions);

        if (portfolios == null)
        {
            return Array.Empty<PortfolioModel>().AsQueryable();
        }

        var modelItems = new List<PortfolioModel>();

        foreach (var portfolio in portfolios)
        {
            var item = new PortfolioModel
            {
                Portfolio = portfolio,
            };

            modelItems.Add(item);
        }

        var res = modelItems?.AsQueryable() ?? Array.Empty<PortfolioModel>().AsQueryable();
        return res;
    }
}
