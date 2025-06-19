using MediatR;

namespace Vertr.TinvestGateway.Contracts.Requests;

// TODO: move to Portfolio
public class HandlePositionRequest : IRequest
{
    public PositionsResponse? Positions { get; init; }
}
