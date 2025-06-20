using MediatR;

namespace Vertr.PortfolioManager.Contracts.Requests;


public class PortfolioChangedRequest : IRequest
{
    public PortfolioSnapshot? Portfolio { get; init; }
}
