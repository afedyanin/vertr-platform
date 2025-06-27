using MediatR;
using Vertr.MarketData.Contracts;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Contracts.Requests;

public class ClosePositionRequest : IRequest<ExecuteOrderResponse>
{
    public Guid RequestId { get; init; }

    public required InstrumentIdentity InstrumentIdentity { get; init; }

    public required PortfolioIdentity PortfolioIdentity { get; init; }
}
