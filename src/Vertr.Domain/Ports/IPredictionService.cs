namespace Vertr.Domain.Ports;

public interface IPredictionService
{
    Task<IEnumerable<(DateTime, TradeAction)>> Predict(
        string symbol,
        CandleInterval interval,
        PredictorType predictor,
        Sb3Algo? algo = null,
        int candlesCount = 20,
        bool completedOnly = false,
        string candlesSource = "tinvest");
}
