namespace Vertr.TinvestGateway.Contracts;

public record class Position(
    string PositionUid,
    string InstrumentUid,
    long Blocked,
    long Balance);
