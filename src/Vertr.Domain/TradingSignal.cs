namespace Vertr.Domain;
public record class TradingSignal
{
    public Guid Id { get; set; }

    public DateTime TimeUtc { get; set; }

    public string Symbol { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public CandleInterval CandleInterval { get; set; }

    public PredictorType PredictorType { get; set; } = PredictorType.Undefined;

    public Sb3Algo Sb3Algo { get; set; } = Sb3Algo.Undefined;

    public string CandlesSource { get; set; } = string.Empty;
}
