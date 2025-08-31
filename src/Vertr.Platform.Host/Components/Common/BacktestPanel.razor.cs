using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.Platform.Host.Components.Models;

namespace Vertr.Platform.Host.Components.Common;

public partial class BacktestPanel
{
    [Parameter]
    public BacktestModel Content { get; set; } = default!;

    [CascadingParameter]
    public FluentDialog? Dialog { get; set; }

    private string _portfolioDeatilsLink => $"/portfolios/details/{Content?.Portfolio?.Id}";
    private bool CancelDiasbled => Content.Backtest.ExecutionState
        is not Backtest.Contracts.ExecutionState.InProgress
        || Content.Backtest.IsCancellationRequested;
    private bool StartDiasbled => Content.Backtest.ExecutionState
        is not Backtest.Contracts.ExecutionState.Created
        and not Backtest.Contracts.ExecutionState.Enqueued;

    private async Task OnCancelAsync()
    {
        if (Dialog == null)
        {
            return;
        }

        Content.DoCancel = true;
        await Dialog.CloseAsync(Content);
    }

    private async Task OnStartAsync()
    {
        if (Dialog == null)
        {
            return;
        }

        Content.DoStart = true;
        await Dialog.CloseAsync(Content);
    }
}
