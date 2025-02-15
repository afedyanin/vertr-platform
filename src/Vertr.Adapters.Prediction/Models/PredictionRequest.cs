namespace Vertr.Adapters.Prediction.Models;

internal record class PredictionRequest
{
    public string Symbol { get; set; } = string.Empty;

    public int Interval { get; set; }

    public string Predictor { get; set; } = string.Empty;

    public string Algo { get; set; } = string.Empty;

    public int CandlesCount { get; set; }

    public bool CompletedCandelsOnly { get; set; }

    public string CandlesSource { get; set; } = string.Empty;

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
