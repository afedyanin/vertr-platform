namespace Vertr.OrderExecution.Contracts;

public record class ClosePositionRequest
    (
    Guid RequestId,
    Guid InstrumentId,
    PortfolioIdentity PortfolioId
    );
