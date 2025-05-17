namespace Vertr.OrderExecution.Contracts;
public record class RevertPositionRequest
    (
    Guid RequestId,
    Guid InstrumentId,
    string AccountId,
    Guid BookId
    );
