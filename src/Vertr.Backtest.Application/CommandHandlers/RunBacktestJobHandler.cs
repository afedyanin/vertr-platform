using Microsoft.Extensions.Logging;
using Vertr.Backtest.Contracts;
using Vertr.Backtest.Contracts.Commands;
using Vertr.Backtest.Contracts.Interfaces;
using Vertr.Platform.Common.Mediator;

namespace Vertr.Backtest.Application.CommandHandlers;

internal class RunBacktestJobHandler : IRequestHandler<RunBacktestJobRequest>
{
    private readonly IBacktestRepository _backtestRepository;
    private readonly ILogger<RunBacktestJobHandler> _logger;

    public RunBacktestJobHandler(
        IBacktestRepository backtestRepository,
        ILogger<RunBacktestJobHandler> logger)
    {
        _backtestRepository = backtestRepository;
        _logger = logger;
    }

    public async Task Handle(RunBacktestJobRequest request, CancellationToken cancellationToken = default)
    {
        var bt = await _backtestRepository.GetById(request.BacktestId);

        if (bt == null)
        {
            _logger.LogError($"Backtest with Id={request.BacktestId} is not found.");
            return;
        }

        if (bt.ExecutionState is not ExecutionState.Created and not ExecutionState.Enqueued)
        {
            _logger.LogError($"Backtest with Id={request.BacktestId} has invalid state to start. State={bt.ExecutionState}");
            return;
        }

        // TODO: Implement backtest logic:
        // - load market data into intraday repo
        // - init strategy

        // Emulate work
        for (int i = 0; i < 100; i++)
        {
            // - iterate md candles
            //      - call strategy handler
            //      - update backtest status and save it 
            await Task.Delay(1000);

            bt = await _backtestRepository.GetById(bt.Id);
            if (bt == null)
            {
                throw new InvalidOperationException($"Cannot find backtest with id={request.BacktestId}");
            }

            if (bt.IsCancellationRequested)
            {
                _logger.LogWarning($"Backtest with Id={request.BacktestId} cancellation requested! State={bt.ExecutionState}");
                await SetCancelled(bt);
                break;
            }

            await SetProgress(bt, $"Processing step #{i + 1}");
        }

        if (bt.ExecutionState == ExecutionState.InProgress)
        {
            await SetCompleted(bt);
        }
    }

    private async Task SetProgress(BacktestRun backtest, string message)
    {
        backtest.ExecutionState = ExecutionState.InProgress;
        backtest.ProgressMessage = message;
        backtest.UpdatedAt = DateTime.UtcNow;
        await _backtestRepository.Save(backtest);
    }

    private async Task SetCancelled(BacktestRun backtest)
    {
        backtest.ExecutionState = ExecutionState.Cancelled;
        backtest.ProgressMessage = "Backtest cancelled.";
        backtest.UpdatedAt = DateTime.UtcNow;
        await _backtestRepository.Save(backtest);
    }

    private async Task SetCompleted(BacktestRun backtest)
    {
        backtest.ExecutionState = ExecutionState.Completed;
        backtest.ProgressMessage = "Backtest completed.";
        backtest.UpdatedAt = DateTime.UtcNow;
        await _backtestRepository.Save(backtest);
    }
}
