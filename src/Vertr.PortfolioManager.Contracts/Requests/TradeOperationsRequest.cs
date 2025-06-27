using MediatR;

namespace Vertr.PortfolioManager.Contracts.Requests;
public class TradeOperationsRequest : IRequest
{
    public TradeOperation[] Operations { get; init; }
}
