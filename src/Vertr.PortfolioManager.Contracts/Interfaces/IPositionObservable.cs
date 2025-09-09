namespace Vertr.PortfolioManager.Contracts.Interfaces;
public interface IPositionObservable
{
    public IObservable<Position> StreamPositions();
}
