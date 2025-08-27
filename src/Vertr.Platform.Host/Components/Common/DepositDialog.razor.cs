using Microsoft.AspNetCore.Components;
using Vertr.Platform.Host.Components.Models;

namespace Vertr.Platform.Host.Components.Common;

public partial class DepositDialog
{
    [Parameter]
    public DepositModel Content { get; set; } = default!;
}
