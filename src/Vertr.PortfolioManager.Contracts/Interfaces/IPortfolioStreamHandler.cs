using Vertr.TinvestGateway.Contracts;

namespace Vertr.PortfolioManager.Contracts.Interfaces;

public interface IPortfolioStreamHandler
{
    public Task Handle(PortfolioResponse? response, CancellationToken token);
}
