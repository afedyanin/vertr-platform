namespace Vertr.Common.Contracts;

public record class TradingSignal
{
    public required string PortfolioName { get; set; }

    public required Instrument Instrument { get; set; }

    public TradingDirection Direction { get; set; }
}
