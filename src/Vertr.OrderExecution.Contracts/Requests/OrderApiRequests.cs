namespace Vertr.OrderExecution.Contracts.Requests;

public record class ExecuteRequest(DateTime Date, Guid InstrumentId, Guid PortfolioId, long Lots, decimal Price);

public record class OpenRequest(DateTime Date, Guid InstrumentId, Guid PortfolioId, long Lots, decimal Price);

public record class CloseRequest(DateTime Date, Guid InstrumentId, Guid PortfolioId, decimal Price);

public record class ReverseRequest(DateTime Date, Guid InstrumentId, Guid PortfolioId, decimal Price);

public record class SignalRequest(DateTime Date, Guid InstrumentId, Guid PortfolioId, long Lots, decimal Price);

