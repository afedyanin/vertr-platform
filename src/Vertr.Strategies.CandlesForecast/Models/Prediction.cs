namespace Vertr.Strategies.CandlesForecast.Models;

public record class Prediction
{
    public required string Predictor { get; set; }

    public Guid InstrumentId { get; set; }

    public decimal? Value { get; set; }
}
