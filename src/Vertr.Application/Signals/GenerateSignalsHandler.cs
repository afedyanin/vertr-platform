using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.Domain;
using Vertr.Domain.Enums;
using Vertr.Domain.Ports;
using Vertr.Domain.Repositories;

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

        foreach (var symbol in request.Symbols)
        {
            tasks.Add(
                HandleSymbol(
                    symbol,
                    request.Interval,
                    request.PredictorType,
                    request.Sb3Algo,
                    cancellationToken));
        }

        await Task.WhenAll(tasks);

        _logger.LogDebug($"Generating trading signals for {request.Symbols.Count()} symbols completed.");
    }

    internal async Task HandleSymbol(
        string symbol,
        CandleInterval interval,
        PredictorType predictorType,
        Sb3Algo sb3Algo,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrEmpty(symbol))
        {
            throw new ArgumentNullException(nameof(symbol));
        }

        try
        {
            var generateSignal = await ShouldGenerateNewSignal(symbol, interval);

            if (!generateSignal)
            {
                _logger.LogDebug($"Skip generating signal for {symbol} {interval}.");
                return;
            }

            var prediction = await _predictionService.Predict(symbol, interval, predictorType, sb3Algo, 100, false, _candlesSource);

            if (prediction == null || !prediction.Any())
            {
                _logger.LogInformation($"No any predictions for {symbol} {interval} predictor={predictorType.Name} algo={sb3Algo.Name}.");
                return;
            }

            (var signalTime, var signalAction) = prediction.Last();

            var tradingSignal = new TradingSignal
            {
                Id = Guid.NewGuid(),
                TimeUtc = signalTime,
                Symbol = symbol,
                Action = signalAction,
                CandleInterval = interval,
                PredictorType = predictorType,
                Sb3Algo = sb3Algo,
                CandlesSource = _candlesSource,
            };

            var res = await _tradingSignalsRepository.Insert(tradingSignal);

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

    internal async Task<bool> ShouldGenerateNewSignal(string symbol, CandleInterval interval)
    {
        var candles = await _tinvestCandlesRepository.GetLast(symbol, interval, 1, false);

        if (candles == null || !candles.Any())
        {
            _logger.LogInformation($"No candles found for {symbol} {interval}.");
            return false;
        }

        var signals = await _tradingSignalsRepository.GetLast(symbol, interval, 1);

        if (signals == null || !signals.Any())
        {
            _logger.LogDebug($"No any signals found for {symbol} {interval}.");
            return true;
        }

        var candle = candles.First();
        var signal = signals.First();

        return signal.TimeUtc < candle.TimeUtc;
    }
}
