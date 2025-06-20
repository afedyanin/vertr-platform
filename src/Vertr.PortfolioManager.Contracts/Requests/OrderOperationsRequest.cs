using MediatR;
using Vertr.OrderExecution.Contracts;

namespace Vertr.PortfolioManager.Contracts.Requests;
public class OrderOperationsRequest : IRequest
{
    public OrderOperation[]? Operations { get; init; }
}
