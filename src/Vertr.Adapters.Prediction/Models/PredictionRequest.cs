namespace Vertr.Adapters.Prediction.Models;

internal record class PredictionRequest
{
    public required string Symbol { get; set; } = string.Empty;

    public int Interval { get; set; }

    public required string Predictor { get; set; }

    public required string Algo { get; set; }

    public int CandlesCount { get; set; }

    public bool CompletedCandelsOnly { get; set; }

    public string CandlesSource { get; set; } = "tinvest"; // "synthetic";

    /*
     class PredictionRequest(BaseModel):
        symbol: str = "SBER"
        interval: int = Interval.CANDLE_INTERVAL_10_MIN.value
        predictor: str = PredictorType.Sb3.value
        algo: str = "dqn"
        candles_count: int = 20
        completed_candles_only: bool = True
        candles_source: str = "tinvest"
     */
}
