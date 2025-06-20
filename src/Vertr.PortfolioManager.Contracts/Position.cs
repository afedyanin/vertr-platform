namespace Vertr.PortfolioManager.Contracts;

public record class Position(
    string PositionId,
    string InstrumentId,
    decimal Blocked,
    decimal Balance);
