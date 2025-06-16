using MediatR;

namespace Vertr.TinvestGateway.Contracts.Requests;

public class HandlePortrolioRequest : IRequest
{
    public PortfolioResponse? Portfolio { get; init; }
}
