using Microsoft.Extensions.Logging;
using Vertr.Backtest.Contracts;
using Vertr.Backtest.Contracts.Commands;
using Vertr.Backtest.Contracts.Interfaces;
using Vertr.MarketData.Contracts;
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
    private readonly ICandlesRepository _candlesRepository;
    private readonly ILogger<RunBacktestJobHandler> _logger;

    public RunBacktestJobHandler(
        IBacktestRepository backtestRepository,
        ICandlesRepository candlesRepository,
        IStrategyMetadataRepository strategyMetadataRepository,
        IStrategyFactory strategyFactory,
        IServiceProvider serviceProvider,
        ILogger<RunBacktestJobHandler> logger)
    {
        _candlesRepository = candlesRepository;
        _backtestRepository = backtestRepository;
        _strategyMetadataRepository = strategyMetadataRepository;
        _strategyFactory = strategyFactory;
        _serviceProvider = serviceProvider;
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

        var candles = await GetCandles(strategy.InstrumentId, bt.From, bt.To);

        var steps = 0;

        try
        {
            _logger.LogInformation($"Starting backtest Id={bt.StrategyId}. Candles={candles.Length}");

            await strategy.OnStart();

            foreach (var candle in candles)
            {
                await strategy.HandleMarketData(candle, cancellationToken);
                steps++;

                if (steps % 10 == 0)
                {
                    bt = await _backtestRepository.GetById(bt.Id);
                    if (bt == null)
                    {
                        var message = $"Cannot find backtest with id={request.BacktestId}";
                        _logger.LogError(message);
                        break;
                    }

                    if (bt.IsCancellationRequested)
                    {
                        _logger.LogWarning($"Backtest with Id={request.BacktestId} cancellation requested! State={bt.ExecutionState}");
                        await SetCancelled(bt);
                        break;
                    }

                    await SetProgress(bt, $"Processing step #{steps}");
                }
            }

            await strategy.OnStop(cancellationToken);

            if (bt != null && bt.ExecutionState == ExecutionState.InProgress)
            {
                await SetCompleted(bt);
            }

            _logger.LogInformation($"Backtest Id={bt?.StrategyId} completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await SetFailed(bt, ex.Message);
        }
    }

    private async Task<Candle[]> GetCandles(Guid instrumentId, DateTime from, DateTime to)
    {
        // TODO: Load history
        var res = await _candlesRepository.Get(instrumentId);
        return res?.ToArray() ?? [];
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
