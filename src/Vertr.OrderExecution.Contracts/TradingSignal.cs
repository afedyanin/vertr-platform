using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Contracts;
public record class TradingSignal
{
    public Guid RequestId { get; init; }

    public required PortfolioIdentity PortfolioIdentity { get; init; }

    public Guid InstrumentId { get; init; }

    public long QtyLots { get; init; }
}
