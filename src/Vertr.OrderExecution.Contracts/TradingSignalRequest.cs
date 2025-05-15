namespace Vertr.OrderExecution.Contracts;

public record class TradingSignalRequest
    (
    Guid RequestId,
    Guid InstrumentId,
    string AccountId,
    long QtyLots
    );
