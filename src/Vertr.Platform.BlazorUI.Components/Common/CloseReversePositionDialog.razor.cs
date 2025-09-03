using Microsoft.AspNetCore.Components;
using Vertr.Platform.BlazorUI.Components.Models;

namespace Vertr.Platform.BlazorUI.Components.Common;

public partial class CloseReversePositionDialog
{
    [Parameter]
    public CloseReversePostionModel Content { get; set; } = default!;

    private bool _selectDateDisabled => !Content.OrderExecutionSimulated;
    private bool _selectPriceDisabled => !Content.OrderExecutionSimulated;
}
