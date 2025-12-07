namespace Vertr.Common.Contracts;

public record class Prediction
{
    public required string Predictor { get; set; }

    public Guid InstrumentId { get; set; }

    public decimal? Price { get; set; }
}
