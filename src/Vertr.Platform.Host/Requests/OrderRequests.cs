namespace Vertr.Platform.Host.Requests;

public record class ExecuteRequest(Guid InstrumentId, Guid SubAccountId, long Lots, decimal Price);

public record class OpenRequest(Guid InstrumentId, Guid SubAccountId, long Lots, decimal Price);

public record class CloseRequest(Guid InstrumentId, Guid SubAccountId, decimal Price);

public record class ReverseRequest(Guid InstrumentId, Guid SubAccountId, decimal Price);

public record class SignalRequest(Guid InstrumentId, Guid SubAccountId, long Lots, decimal Price);

