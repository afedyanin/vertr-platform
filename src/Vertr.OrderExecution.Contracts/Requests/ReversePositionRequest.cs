using MediatR;
using Vertr.MarketData.Contracts;

namespace Vertr.OrderExecution.Contracts.Requests;

public class ReversePositionRequest : IRequest<ExecuteOrderResponse>
{
    public Guid RequestId { get; init; }

    public required InstrumentIdentity InstrumentIdentity { get; init; }

    public required PortfolioIdentity PortfolioIdentity { get; init; }
}
