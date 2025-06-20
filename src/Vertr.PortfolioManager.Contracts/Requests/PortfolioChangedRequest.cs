using MediatR;

namespace Vertr.PortfolioManager.Contracts.Requests;


public class PortfolioChangedRequest : IRequest
{
    public PortfolioResponse? Portfolio { get; init; }
}
