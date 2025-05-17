namespace Vertr.OrderExecution.Contracts;

public record class TradingSignalRequest
    (
    Guid RequestId,
    Guid InstrumentId,
    long QtyLots,
    PortfolioIdentity PortfolioId
    );
