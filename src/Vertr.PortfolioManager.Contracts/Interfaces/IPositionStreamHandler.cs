using Vertr.TinvestGateway.Contracts;

namespace Vertr.PortfolioManager.Contracts.Interfaces;

public interface IPositionStreamHandler
{
    public Task Handle(PositionsResponse? response, CancellationToken token);

}
