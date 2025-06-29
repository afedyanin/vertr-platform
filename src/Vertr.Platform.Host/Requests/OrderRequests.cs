namespace Vertr.Platform.Host.Requests;

public record class ExecuteRequest(Guid InstrumentId, string AccountId, Guid SubAccountId, long Lots);

public record class OpenRequest(Guid InstrumentId, string AccountId, Guid SubAccountId, long Lots);

public record class CloseRequest(Guid InstrumentId, string AccountId, Guid SubAccountId);

public record class ReverseRequest(Guid InstrumentId, string AccountId, Guid SubAccountId);

public record class SignalRequest(Guid InstrumentId, string AccountId, Guid SubAccountId, long Lots);

