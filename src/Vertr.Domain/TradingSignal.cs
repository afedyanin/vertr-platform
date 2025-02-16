namespace Vertr.Domain;
public record class TradingSignal
{
    public Guid Id { get; set; }

    public DateTime TimeUtc { get; set; }

    public string Symbol { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public CandleInterval CandleInterval { get; set; }

    public PredictorType? PredictorType { get; set; }

    public Sb3Algo? Sb3Algo { get; set; }

    public string CandlesSource { get; set; } = string.Empty;
}
