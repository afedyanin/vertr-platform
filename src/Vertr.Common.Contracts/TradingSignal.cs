namespace Vertr.Common.Contracts;

public record class TradingSignal
{
    public required string Predictor { get; set; }

    public required Instrument Instrument { get; set; }

    public TradingDirection Direction { get; set; }
}
