using Microsoft.AspNetCore.Components;

namespace Vertr.Platform.Host.Components.Pages;

public partial class CandlesHistory
{
    [Parameter]
    public Guid? HistoryItemId { get; set; }

}
