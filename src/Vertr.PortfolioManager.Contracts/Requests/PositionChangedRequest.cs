using MediatR;
using Vertr.TinvestGateway.Contracts;

namespace Vertr.PortfolioManager.Contracts.Requests;

public class PositionChangedRequest : IRequest
{
    public PositionsResponse? Positions { get; init; }
}
