using Vertr.CommandLine.Models.Requests.Predictor;

namespace Vertr.CommandLine.Models.BackTest;

public class BackTestParams
{
    public Guid PortfolioId { get; init; }

    public required string Symbol { get; init; }

    public required string CurrencyCode { get; init; }

    public int Steps { get; init; }

    public int Skip { get; init; }

    public decimal OpenPositionQty { get; init; } = 100;

    public decimal ComissionPercent { get; init; } = 0.0005m;

    public decimal PriceThreshold { get; init; } = 0.0010m;

    public DateOnly? Intraday { get; init; }

    public PredictorType Predictor { get; init; }

    public int PredictionRequestCandlesCount { get; init; } = 1;
}