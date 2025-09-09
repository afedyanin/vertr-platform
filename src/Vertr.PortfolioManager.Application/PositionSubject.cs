using System.Reactive.Subjects;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.PortfolioManager.Application;

internal class PositionSubject : IPositionObservable, IPositionDataHandler, IDisposable
{
    private readonly Subject<Position> _positionSubject;

    public IObservable<Position> StreamPositions() => _positionSubject;

    public PositionSubject()
    {
        _positionSubject = new Subject<Position>();
    }

    public Task HandlePosition(Position position)
    {
        _positionSubject.OnNext(position);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _positionSubject?.Dispose();
    }
}
