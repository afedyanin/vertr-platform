using Vertr.Common.Contracts;

namespace Vertr.Strategies.CandlesForecast.Models;

public record struct PredictionSampleInfo
{
    public DateTime From { get; set; }

    public DateTime To { get; set; }

    public int Count { get; set; }

    public BasicStats ClosePriceStats { get; set; }
}
