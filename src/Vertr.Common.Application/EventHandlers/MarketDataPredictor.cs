using Disruptor;
using Microsoft.Extensions.Logging;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.EventHandlers;

internal sealed class MarketDataPredictor : IEventHandler<CandlestickReceivedEvent>
{
    private readonly ILogger<MarketDataPredictor> _logger;

    public MarketDataPredictor(ILogger<MarketDataPredictor> logger)
    {
        _logger = logger;
    }

    public void OnEvent(CandlestickReceivedEvent data, long sequence, bool endOfBatch)
    {
        // Collect required market data
        // Get Predictions from prediction engine.
        // Save predictions to event data

        var prediction = new Prediction
        {
            Predictor = "RandomWalk",
            InstrumentId = data.Candle!.InstrumentId,
            Price = GetRandomPrice(data.Candle!)
        };

        data.Predictions.Add(prediction);

        _logger.LogInformation("MarketDataPredictor executed.");
    }

    private decimal GetRandomPrice(Candle candle)
    {
        var prices = new decimal[]
        {
            candle.Open,
            candle.High,
            candle.Low,
            candle.Close,
        };

        var count = prices.Length;
        var avg = prices.Average();
        var sum = prices.Sum(d => (d - avg) * (d - avg));
        var dev = Math.Sqrt((double)sum / count);
        var diff = dev * GetRandomDirection();

        return (decimal)((double)avg + diff);
    }

    private static int GetRandomDirection()
        => Random.Shared.Next(0, 2) > 0 ? 1 : -1;
}
