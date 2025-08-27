namespace Vertr.Platform.Host.Requests;

public record class ExecuteRequest(Guid InstrumentId, Guid PortfolioId, long Lots, decimal Price);

public record class OpenRequest(DateTime Date, Guid InstrumentId, Guid PortfolioId, long Lots, decimal Price);

public record class CloseRequest(Guid InstrumentId, Guid PortfolioId, decimal Price);

public record class ReverseRequest(Guid InstrumentId, Guid PortfolioId, decimal Price);

public record class SignalRequest(Guid InstrumentId, Guid PortfolioId, long Lots, decimal Price);

