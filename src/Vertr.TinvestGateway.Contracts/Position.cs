namespace Vertr.TinvestGateway.Contracts;

public record class Position(
    string PositionUid,
    string InstrumentUid,
    decimal Blocked,
    decimal Balance);
