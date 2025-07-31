using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.Platform.Host.Components.Models;

namespace Vertr.Platform.Host.Components.Common;

public partial class InstrumentScreener
{
    [Parameter]
    public SimplePerson Content { get; set; } = default!;

    [CascadingParameter]
    public FluentDialog? Dialog { get; set; }

    private FluentSearch? searchTest;

    private string? searchValue = string.Empty;

    private List<string> searchResults = defaultResults();

    private static string defaultResultsText = "no results";
    private static List<string> defaultResults()
    {
        return new() { defaultResultsText };
    }

    private void HandleSearchInput()
    {
        if (string.IsNullOrWhiteSpace(searchValue))
        {
            searchResults = defaultResults();
            searchValue = string.Empty;
        }
        else
        {
            string searchTerm = searchValue.ToLower();

            if (searchTerm.Length > 0)
            {
                List<string> temp = searchData.Where(str => str.ToLower().Contains(searchTerm)).Select(str => str).ToList();
                if (temp.Count() > 0)
                {
                    searchResults = temp;
                }
            }
        }
    }

    private List<string> searchData = new()
    {
        "Alabama",
        "Alaska",
        "Arizona",
        "Arkansas",
        "California",
        "Colorado",
        "Connecticut",
        "Delaware",
        "Florida",
        "Georgia",
        "Hawaii",
        "Idaho",
        "Illinois",
        "Indiana",
        "Iowa",
        "Kansas",
        "Kentucky",
        "Louisiana",
        "Maine",
        "Maryland",
        "Massachussets",
        "Michigain",
        "Minnesota",
        "Mississippi",
        "Missouri",
        "Montana",
        "Nebraska",
        "Nevada",
        "New Hampshire",
        "New Jersey",
        "New Mexico",
        "New York",
        "North Carolina",
        "North Dakota",
        "Ohio",
        "Oklahoma",
        "Oregon",
        "Pennsylvania",
        "Rhode Island",
        "South Carolina",
        "South Dakota",
        "Texas",
        "Tennessee",
        "Utah",
        "Vermont",
        "Virginia",
        "Washington",
        "Wisconsin",
        "West Virginia",
        "Wyoming"
    };

}
