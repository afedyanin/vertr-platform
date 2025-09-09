namespace Vertr.PortfolioManager.Contracts.Interfaces;
public interface IPositionDataHandler
{
    public Task HandlePosition(Position position);
}
