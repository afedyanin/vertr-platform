using MediatR;

namespace Vertr.TinvestGateway.Contracts.Requests;

// TODO: move to Portfolio

public class HandlePortrolioRequest : IRequest
{
    public PortfolioResponse? Portfolio { get; init; }
}
