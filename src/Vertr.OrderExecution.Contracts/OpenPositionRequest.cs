namespace Vertr.OrderExecution.Contracts;
public record class OpenPositionRequest
    (
    Guid RequestId,
    Guid InstrumentId,
    long QtyLots,
    PortfolioIdentity PortfolioId
    );
