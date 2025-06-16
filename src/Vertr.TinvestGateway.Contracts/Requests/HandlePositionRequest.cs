using MediatR;

namespace Vertr.TinvestGateway.Contracts.Requests;
public class HandlePositionRequest : IRequest
{
    public PositionsResponse? Positions { get; init; }
}
