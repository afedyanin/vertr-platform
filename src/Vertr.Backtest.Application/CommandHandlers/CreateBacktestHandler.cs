using Microsoft.Extensions.Logging;
using Vertr.Backtest.Contracts;
using Vertr.Backtest.Contracts.Commands;
using Vertr.Backtest.Contracts.Interfaces;
using Vertr.Platform.Common.Jobs;
using Vertr.Platform.Common.Mediator;

namespace Vertr.Backtest.Application.CommandHandlers;

internal class CreateBacktestHandler : IRequestHandler<CreateBacktestRequest, CreateBacktestResponse>
{
    private readonly IBacktestRepository _backtestRepository;
    private readonly IJobScheduler _jobScheduler;
    private readonly ILogger<CreateBacktestHandler> _logger;

    public CreateBacktestHandler(
        IBacktestRepository backtestRepository,
        IJobScheduler jobScheduler,
        ILogger<CreateBacktestHandler> logger)
    {
        _backtestRepository = backtestRepository;
        _jobScheduler = jobScheduler;
        _logger = logger;
    }

    public async Task<CreateBacktestResponse> Handle(
        CreateBacktestRequest request,
        CancellationToken cancellationToken = default)
    {
        var bt = new BacktestRun
        {
            Id = Guid.NewGuid(),
            From = request.From,
            To = request.To,
            CreatedAt = DateTime.UtcNow,
            Description = request.Description,
            StrategyId = request.StrategyId,
            PortfolioId = request.PortfolioId,
            ExecutionState = ExecutionState.Created,
        };

        var saved = await _backtestRepository.Save(bt);

        if (!saved)
        {
            throw new InvalidOperationException($"Cannot save backtest instance.");
        }

        _logger.LogDebug($"Backtest created. BacktestId={bt.Id}");

        if (request.StartImmediately)
        {
            var runCommand = new RunBacktestJobRequest
            {
                BacktestId = bt.Id,
            };

            var jobId = _jobScheduler.Enqueue(runCommand, cancellationToken);

            bt.ExecutionState = ExecutionState.Enqueued;
            saved = await _backtestRepository.Save(bt);

            if (!saved)
            {
                throw new InvalidOperationException($"Cannot save backtest instance.");
            }

            _logger.LogInformation($"Backtest enqueued. BacktestId={bt.Id} JobId={jobId}");
        }

        var response = new CreateBacktestResponse
        {
            BacktestId = bt.Id,
        };

        return response;
    }
}
