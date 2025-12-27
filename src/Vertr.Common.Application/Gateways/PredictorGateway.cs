using System.Diagnostics;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;
using Vertr.Common.ForecastClient;
using Vertr.Common.ForecastClient.Models;

namespace Vertr.Common.Application.Gateways;

internal class PredictorGateway : IPredictorGateway
{
    private class ParsedPredictors
    {
        public HashSet<string> StatsPredictors { get; } = [];
        public HashSet<string> MlPredictors { get; } = [];
        public HashSet<string> NeuralPredictors { get; } = [];

        public ParsedPredictors(string[] predictors)
        {
            foreach (var predictor in predictors)
            {
                if (Predictors.Stats.AllKeys.Contains(predictor))
                {
                    StatsPredictors.Add(predictor);
                    continue;
                }

                if (Predictors.Ml.AllKeys.Contains(predictor))
                {
                    StatsPredictors.Add(predictor);
                    continue;
                }

                if (Predictors.Neural.AllKeys.Contains(predictor))
                {
                    StatsPredictors.Add(predictor);
                    continue;
                }
            }
        }
    }

    private readonly IVertrForecastClient _forecastClient;

    public PredictorGateway(IVertrForecastClient forecastClient)
    {
        _forecastClient = forecastClient;
    }

    public async Task<Prediction[]> Predict(string[] predictors, Candle[] candles)
    {
        var predictorsByCategory = new ParsedPredictors(predictors);
        var series = CreateSeries(candles);

        var predictions = new List<Prediction>();

        if (predictorsByCategory.StatsPredictors.Any())
        {
            var request = new ForecastRequest
            {
                Models = predictorsByCategory.StatsPredictors.ToArray(),
                Series = series,
            };

            var statsForecast = await _forecastClient.ForecastStats(request);
            predictions.AddRange(CreatePredictions(statsForecast));
        }

        if (predictorsByCategory.MlPredictors.Any())
        {
            var request = new ForecastRequest
            {
                Models = predictorsByCategory.StatsPredictors.ToArray(),
                Series = series,
            };

            var mlForecast = await _forecastClient.ForecastMl(request);
            predictions.AddRange(CreatePredictions(mlForecast));
        }

        if (predictorsByCategory.NeuralPredictors.Any())
        {
            foreach (var predictor in predictorsByCategory.NeuralPredictors)
            {
                var prediction = await PredictNeural(predictor, series);

                if (prediction != null)
                {
                    predictions.Add(prediction);
                }
            }
        }

        return [.. predictions];
    }

    private async Task<Prediction?> PredictNeural(string model, SeriesItem[] series)
    {
        Debug.Assert(series != null);
        var instrumentId = Guid.Parse(series[0].Ticker);

        var value = model switch
        {
            Predictors.Neural.AutoLSTM => await _forecastClient.AutoLSTM(series),
            Predictors.Neural.AutoRNN => await _forecastClient.AutoRNN(series),
            Predictors.Neural.AutoMLP => await _forecastClient.AutoMLP(series),
            Predictors.Neural.AutoDeepAR => await _forecastClient.AutoDeepAR(series),
            Predictors.Neural.AutoDeepNPTS => await _forecastClient.AutoDeepNPTS(series),
            Predictors.Neural.AutoKAN => await _forecastClient.AutoKAN(series),
            Predictors.Neural.AutoTFT => await _forecastClient.AutoTFT(series),
            Predictors.Neural.AutoTimesNet => (decimal?)await _forecastClient.AutoTimesNet(series),
            _ => null,
        };

        return value == null ? null : new Prediction
        {
            Predictor = model,
            InstrumentId = instrumentId,
            Value = value
        };
    }

    private static Prediction CreatePrediction(ForecastItem forecastItem)
        => new Prediction
        {
            InstrumentId = Guid.Parse(forecastItem.Ticker!),
            Predictor = forecastItem.Model!,
            Value = forecastItem.Value,
        };

    private static Prediction[] CreatePredictions(ForecastItem[] forecastItems)
        => [.. forecastItems.Select(CreatePrediction)];

    private static SeriesItem[] CreateSeries(Candle[] candles)
    {
        var items = new SeriesItem[candles.Length];

        for (var i = 0; i < items.Length; i++)
        {
            var candle = candles[i]!;
            items[i] = new SeriesItem
            {
                Ticker = candle.InstrumentId.ToString(),
                Time = candle.TimeUtc.ToString("yyyy-MM-ddTHH:mm:ss"),
                Value = candle.Close
            };
        }

        return items;
    }
}
