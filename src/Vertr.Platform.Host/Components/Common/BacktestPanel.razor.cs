using Microsoft.AspNetCore.Components;
using Vertr.Platform.Host.Components.Models;

namespace Vertr.Platform.Host.Components.Common;

public partial class BacktestPanel
{
    [Parameter]
    public BacktestModel Content { get; set; } = default!;

    [Inject]
    private IHttpClientFactory _httpClientFactory { get; set; }
}
