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

    private async Task AddPortfolioAsync()
    {
        var model = new PortfolioModel()
        {
            Portfolio = new Portfolio
            {
                Id = Guid.NewGuid(),
                Name = "Portfolio",
                IsBacktest = false,
                UpdatedAt = DateTime.UtcNow,
            }
        };

        await OpenDialogAsync(model);
    }

    private async Task OpenDialogAsync(PortfolioModel portfolioModel)
    {
        _dialog = await DialogService.ShowDialogAsync<PortfolioDialog>(portfolioModel, new DialogParameters<PortfolioModel>()
        {
            Content = portfolioModel,
            Alignment = HorizontalAlignment.Right,
            Title = "Add new portfolio",
            PrimaryAction = "Save",
            SecondaryAction = "Cancel",
            Width = "400px",
        });

        var result = await _dialog.Result;

        if (result.Cancelled)
        {
            // TODO: implement discard changes
            _portfolios = await InitPortfolios();
            await dataGrid.RefreshDataAsync(force: true);
            return;
        }

        if (result.Data is null)
        {
            return;
        }

        if (result.Data is not PortfolioModel model)
        {
            return;
        }

        var saved = await SavePortfolio(model);
        if (saved)
        {
            DemoLogger.WriteLine($"Portfolio {model.Portfolio.Name} Id={model.Portfolio.Id} saved.");
        }
        else
        {
            DemoLogger.WriteLine($"Saving portfolio {model.Portfolio.Name} Id={model.Portfolio.Id} FAILED!");
        }

        _portfolios = await InitPortfolios();
        await dataGrid.RefreshDataAsync(force: true);
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

    private async Task<bool> SavePortfolio(PortfolioModel portfolioModel)
    {
        try
        {
            using var apiClient = _httpClientFactory.CreateClient("backend");
            var content = JsonContent.Create(portfolioModel.Portfolio);
            var message = await apiClient.PostAsync("api/portfolios", content);
            message.EnsureSuccessStatusCode();
            return true;
        }
        catch
        {
            // TODO: Use toast service
            return false;
        }
    }
}
