using Vertr.Common.Contracts;

namespace Vertr.Strategies.FuturesArbitrage.Models;

public record class TradingStatsInfo
{
    public int Sequence { get; init; }

    public required BaseAssetInfo BaseAsset { get; init; }

    public required DerivedAssetInfo DerivedAsset { get; init; }

    public required OrderExecutionInfo OrderExecutionInfo { get; init; }

    public TradeInfo? TradeInfo { get; init; }
}

public record class BaseAssetInfo
{
    public Guid InstrumentId { get; init; }
    public DateTime UpdatedAt { get; set; }
    public decimal MaxBid { get; init; }
    public decimal MinAsk { get; init; }
    public TradingDirection Direction { get; init; }
}

public record class DerivedAssetInfo
{
    public Guid InstrumentId { get; init; }
    public Guid? PortfolioId { get; init; }
    public DateTime UpdatedAt { get; set; }
    public decimal FairPrice { get; init; }
    public decimal MaxBid { get; init; }
    public decimal MinAsk { get; init; }
    public double Threshold { get; init; }
    public TradingDirection Direction { get; init; }
}
public record class OrderExecutionInfo
{
    public Guid InstrumentId { get; init; }
    public Guid PortfolioId { get; init; }
    public Guid OrderRequestId { get; init; }
    public string? OrderId { get; init; }
}

public record class TradeInfo
{
    public Guid InstrumentId { get; init; }
    public string? OrderId { get; init; }
    public string TradeId { get; init; } = string.Empty;
    public TradingDirection Direction { get; init; }
    public DateTime ExecutionTime { get; init; }
    public decimal Price { get; init; }
    public long Quantity { get; init; }
}
