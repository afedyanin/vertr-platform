namespace Vertr.OrderExecution.Contracts;
public record class RevertPositionRequest
    (
    Guid RequestId,
    Guid InstrumentId,
    PortfolioIdentity PortfolioId
    );
