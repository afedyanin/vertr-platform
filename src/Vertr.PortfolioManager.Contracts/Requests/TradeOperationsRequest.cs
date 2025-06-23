using MediatR;
using Vertr.OrderExecution.Contracts;

namespace Vertr.PortfolioManager.Contracts.Requests;
public class TradeOperationsRequest : IRequest
{
    public TradeOperation[]? Operations { get; init; }
}
