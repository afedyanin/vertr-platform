namespace Vertr.OrderExecution.Contracts;
public record class OpenPositionRequest
    (
    Guid RequestId,
    Guid InstrumentId,
    string AccountId,
    long QtyLots,
    Guid BookId
    );
