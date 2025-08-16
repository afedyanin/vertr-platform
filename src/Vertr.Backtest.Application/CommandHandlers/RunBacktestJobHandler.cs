using Microsoft.Extensions.Logging;
using Vertr.Backtest.Contracts;
using Vertr.Backtest.Contracts.Commands;
using Vertr.Backtest.Contracts.Interfaces;
using Vertr.MarketData.Contracts.Extensions;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.Platform.Common.Mediator;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Backtest.Application.CommandHandlers;

internal class RunBacktestJobHandler : IRequestHandler<RunBacktestJobRequest>
{
    private readonly IBacktestRepository _backtestRepository;
    private readonly IStrategyMetadataRepository _strategyMetadataRepository;
    private readonly IStrategyFactory _strategyFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ICandlesHistoryLoader _candlesHistoryLoader;

    private readonly ILogger<RunBacktestJobHandler> _logger;

    public RunBacktestJobHandler(
        IBacktestRepository backtestRepository,
        IStrategyMetadataRepository strategyMetadataRepository,
        IStrategyFactory strategyFactory,
        IServiceProvider serviceProvider,
        ICandlesHistoryLoader candlesHistoryLoader,
        ILogger<RunBacktestJobHandler> logger)
    {
        _backtestRepository = backtestRepository;
        _strategyMetadataRepository = strategyMetadataRepository;
        _strategyFactory = strategyFactory;
        _serviceProvider = serviceProvider;
        _candlesHistoryLoader = candlesHistoryLoader;
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

        var strategyMeta = await _strategyMetadataRepository.GetById(bt.StrategyId);

        if (strategyMeta == null)
        {
            _logger.LogError($"Cannot find strategy with Id={bt.StrategyId}.");
            return;
        }

        strategyMeta.SubAccountId = bt.SubAccountId;
        var strategy = _strategyFactory.Create(strategyMeta, _serviceProvider, bt.Id);

        try
        {
            _logger.LogInformation($"Starting backtest Id={bt.Id}.");

            await strategy.OnStart();

            var day = DateOnly.FromDateTime(bt.From);
            var dayTo = DateOnly.FromDateTime(bt.To);

            while (day <= dayTo)
            {
                var canContinue = await DoBacktestDayStep(
                    strategy,
                    bt.Id,
                    day,
                    bt.From,
                    bt.To,
                    cancellationToken);

                if (!canContinue)
                {
                    break;
                }

                day.AddDays(1);
            }

            await strategy.OnStop(cancellationToken);

            if (bt != null && bt.ExecutionState == ExecutionState.InProgress)
            {
                await SetCompleted(bt);
            }

            _logger.LogInformation($"Backtest Id={bt?.Id} completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await SetFailed(bt, ex.Message);
        }
    }

    private async Task<bool> DoBacktestDayStep(
        IStrategy strategy,
        Guid backtestId,
        DateOnly day,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var bt = await _backtestRepository.GetById(backtestId);

        if (bt == null)
        {
            var message = $"Cannot find backtest with id={backtestId}";
            _logger.LogError(message);
            return false;
        }

        if (bt.IsCancellationRequested)
        {
            _logger.LogWarning($"Backtest with Id={backtestId} cancellation requested. State={bt.ExecutionState}");
            await SetCancelled(bt);
            return false;
        }

        await SetProgress(bt, $"Processing day: {day:dd-MMM-yyy}.");

        // force reload for intraday candles
        var shouldForce = DateOnly.FromDateTime(DateTime.UtcNow) == day;

        var history = await _candlesHistoryLoader.GetCandlesHistory(
            strategy.InstrumentId,
            day,
            force: shouldForce);

        if (history == null)
        {
            return true;
        }

        var candles = history.GetCandles();

        foreach (var candle in candles)
        {
            if (candle.TimeUtc < from || candle.TimeUtc > to)
            {
                continue;
            }

            await strategy.HandleMarketData(candle, cancellationToken);
        }

        return true;
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

    private async Task SetFailed(BacktestRun? backtest, string errorMessage)
    {
        if (backtest == null)
        {
            return;
        }

        backtest.ExecutionState = ExecutionState.Failed;
        backtest.ProgressMessage = errorMessage;
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
