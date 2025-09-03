using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.MarketData.Contracts;

namespace Vertr.Platform.BlazorUI.Components.Common;

public partial class InstrumentScreener
{
    [Parameter]
    public Instrument Content { get; set; } = default!;

    [CascadingParameter]
    public FluentDialog? Dialog { get; set; }

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }

    private string? searchValue = string.Empty;

    private List<Instrument> searchResults = defaultResults();

    private static List<Instrument> defaultResults() => [];

    private async Task HandleSearchInput()
    {
        if (string.IsNullOrWhiteSpace(searchValue))
        {
            searchResults = defaultResults();
            searchValue = string.Empty;
        }
        else
        {
            var searchTerm = searchValue.ToLower();

            if (searchTerm.Length > 0)
            {
                using var apiClient = _httpClientFactory.CreateClient("backend");
                var items = await apiClient.GetFromJsonAsync<Instrument[]>($"api/tinvest/instrument-find/{searchTerm}");

                if (items != null && items.Count() > 0)
                {
                    searchResults = [.. items.Take(10)];
                }
                else
                {
                    searchResults = defaultResults();
                }
            }
        }
    }
    private void HandleOptionChanged(Instrument item)
    {
        Content.Id = item.Id;
        Content.Name = item.Name;
    }
}
