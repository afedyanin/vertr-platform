using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models.Requests.Orders;

namespace Vertr.CommandLine.Models.Requests.Predictor;

public class PredictionRequest : IRequest<PredictionResponse>
{
    public DateTime Time { get; init; }

    public required string Symbol { get; init; }

    public required PredictorType Predictor { get; init; }

    public int CandlesCount { get; init; } = 1;
}

public class PredictionResponse : ResponseBase
{
    public Direction? Signal { get; init; }

    public decimal? PredictedPrice { get; init; }

    public Candle? LastCandle { get; init; }
}

public enum PredictorType
{
    None = 0,
    AutoArima = 10,
    Garch = 11,
    RandomWalkWithDrift = 12,
    Naive = 13,
    HistoricAverage = 14,
}

public static class PredictionContextKeys
{
    public const string Signal = "Signal";
    public const string PredictedPrice = "PredictedPrice";
    public const string LastCandle = "LastCandle";
    public const string Candles = "Candles";
}