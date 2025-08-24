using Microsoft.AspNetCore.Components;
using Vertr.Platform.Host.Components.Models;

namespace Vertr.Platform.Host.Components.Common;

public partial class PortfolioDialog
{
    [Parameter]
    public PortfolioModel Content { get; set; } = default!;
}
