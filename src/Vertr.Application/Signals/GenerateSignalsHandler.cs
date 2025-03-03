using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.Domain;
using Vertr.Domain.Ports;
using Vertr.Domain.Repositories;
using Vertr.Domain.Settings;

namespace Vertr.Application.Signals;

internal class GenerateSignalsHandler : IRequestHandler<GenerateSignalsRequest>
{
    private const string _candlesSource = "tinvest";

    private readonly IPredictionService _predictionService;
    private readonly ITradingSignalsRepository _tradingSignalsRepository;
    private readonly ITinvestCandlesRepository _tinvestCandlesRepository;

    private readonly ILogger<GenerateSignalsHandler> _logger;

    public GenerateSignalsHandler(
        IPredictionService predictionService,
        ITradingSignalsRepository tradingSignalsRepository,
        ITinvestCandlesRepository tinvestCandlesRepository,
        ILogger<GenerateSignalsHandler> logger)
    {
        _predictionService = predictionService;
        _tradingSignalsRepository = tradingSignalsRepository;
        _tinvestCandlesRepository = tinvestCandlesRepository;
        _logger = logger;
    }

    public async Task Handle(GenerateSignalsRequest request, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();

        foreach (var strategy in request.Strategies)
        {
            tasks.Add(HandleStrategy(strategy, cancellationToken));
        }

        await Task.WhenAll(tasks);

        _logger.LogDebug($"Generating trading signals for {request.Strategies.Count()} strategies completed.");
    }

    internal async Task HandleStrategy(
        StrategySettings strategySettings,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var generateSignal = await ShouldGenerateNewSignal(strategySettings, cancellationToken);

            if (!generateSignal)
            {
                _logger.LogDebug($"Skip generating signal for {strategySettings}.");
                return;
            }

            var prediction = await _predictionService.Predict(strategySettings, 100, false, _candlesSource);

            if (prediction == null || !prediction.Any())
            {
                _logger.LogInformation($"No any predictions for {strategySettings}.");
                return;
            }

            (var signalTime, var signalAction) = prediction.Last();

            var tradingSignal = new TradingSignal
            {
                Id = Guid.NewGuid(),
                TimeUtc = signalTime,
                Symbol = strategySettings.Symbol,
                Action = signalAction,
                CandleInterval = strategySettings.Interval,
                PredictorType = strategySettings.PredictorType,
                Sb3Algo = strategySettings.Sb3Algo,
                CandlesSource = _candlesSource,
                QuantityLots = strategySettings.QuantityLots,
            };

            var res = await _tradingSignalsRepository.Insert(tradingSignal, cancellationToken);

            if (res > 0)
            {
                _logger.LogInformation($"New trading signal saved: {tradingSignal}");
            }
            else
            {
                _logger.LogError($"Cannot save trading signal: {tradingSignal}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Trading signal generation error.");
        }
    }

    internal async Task<bool> ShouldGenerateNewSignal(StrategySettings strategySettings, CancellationToken cancellationToken)
    {
        var candles = await _tinvestCandlesRepository.GetLast(
            strategySettings.Symbol,
            strategySettings.Interval,
            1,
            false);

        if (candles == null || !candles.Any())
        {
            _logger.LogInformation($"No candles found for {strategySettings}.");
            return false;
        }

        var signal = await _tradingSignalsRepository.GetLast(strategySettings, cancellationToken);

        if (signal == null)
        {
            _logger.LogDebug($"No any signals found for {strategySettings}.");
            return true;
        }

        var candle = candles.First();

        return signal.TimeUtc < candle.TimeUtc;
    }
}
